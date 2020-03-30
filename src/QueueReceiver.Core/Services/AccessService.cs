using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Threading.Tasks;
using QueueReceiver.Core.Properties;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace QueueReceiver.Core.Services
{
    public class AccessService : IAccessService
    {
        private readonly IPersonService _personService;
        private readonly IPersonProjectService _personProjectService;
        private readonly IPlantService _plantService;
        private readonly ILogger<AccessService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AccessService(
            IPersonService personService,
            IPersonProjectService personProjectService,
            IPlantService plantService,
            ILogger<AccessService> logger,
            IUnitOfWork unitOfWork)
        {
            _personService = personService;
            _personProjectService = personProjectService;
            _plantService = plantService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleRequestAsync(AccessInfo accessInfo)
        {
            string? plantId = await _plantService.GetPlantIdAsync(accessInfo.PlantOid);

            if (plantId == null)
            {
                _logger.LogInformation(Resources.GroupDoesNotExist);
                return;
            }

            if (MessageHasNoRelevantData(accessInfo))
            {
                return;
            }

            // Set person related DbContextSettings
            await _personService.UpdateDbContextPersonSettings();

            _logger.LogInformation($"Updating access for {accessInfo.Members.Count} members to plant {plantId}");

            await UpdateMemberInfo(accessInfo.Members);

            await UpdateMemberAccess(accessInfo.Members, plantId);
        }

        public async Task UpdateMemberInfo(List<Member> members)
        {
            var tasks = members.Select(async member =>
            {
                if (member.ShouldVoid)
                {
                   await _personService.UpdateWithOidIfNotFound(member.UserOid);
                }
                else
                {
                    await _personService.CreateIfNotExist(member.UserOid);
                }
            });

            await Task.WhenAll(tasks);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMemberAccess(List<Member> members, string plantId)
        {
            var tasks = members.Select(async member =>
            {
                if (member.ShouldVoid)
                {
                    await RemoveAccess(member.UserOid, plantId);
                }
                else
                {
                    await GiveAccess(member.UserOid, plantId);
                }
            });

            await Task.WhenAll(tasks);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task RemoveAccess(string userOid, string plantId)
        {
            Person? person = await _personService.FindPersonByOidAsync(userOid);

            if (person == null)
            {
               // _personService.HandleUserIfDeleted(userOid);
                //TODO Check if deleted, then erase oid if not found
                _logger.LogInformation(Resources.PersonDoesNotExist);
                return;
            }

            _logger.LogInformation(string.Format(
                CultureInfo.InvariantCulture,
                Resources.RemoveAccess, person.Id, plantId));
            
            await _personProjectService.RemoveAccessToPlant(person.Id, plantId);
        }

        private async Task GiveAccess(string userOid, string plantId)
        {
            long personId = await _personService.GetPersonIdByOidAsync(userOid);

            if (personId == 0)
            {
                _logger.LogError(Resources.PersonWasNotFoundOrCreated, userOid);
                return;
            }

            _logger.LogInformation(Resources.AddAccess, personId, plantId);
            await _personProjectService.GiveProjectAccessToPlantAsync(personId, plantId);
        }

        private static bool MessageHasNoRelevantData(AccessInfo accessInfo)
            => accessInfo.Members == null;
    }
}