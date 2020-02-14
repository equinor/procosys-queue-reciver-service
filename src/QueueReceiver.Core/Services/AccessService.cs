using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using QueueReceiver.Core.Properties;
using System.Globalization;
using System.Collections.Generic;

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
            ILogger<AccessService> logger, IUnitOfWork unitOfWork)
        {
            _personService = personService;
            _personProjectService = personProjectService;
            _plantService = plantService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleRequest(AccessInfo accessInfo)
        {
            string? plantId = await _plantService.GetPlantId(accessInfo.PlantOid);
           foreach(Member member in accessInfo.Members)
            {
                if (plantId == null)
                {
                    _logger.LogInformation(Resources.GroupDoesNotExist);
                    return;
                }

                if (MessageHasNoRelevantData(accessInfo))
                {
                    return;
                }

                await UpdateMemberInfo(member);
            }    

            await UpdateMemberAccess(accessInfo.Members, plantId);
        }

        public async Task UpdateMemberAccess(List<Member> members, string plantId)
        {
            var syncAccessTasks = members.Select(async member =>
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

            await Task.WhenAll(syncAccessTasks);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMemberInfo(Member member)
        {
                if (member.ShouldVoid)
                {
                    await _personService.UpdateWithOidIfNotFound(member.UserOid);
                }
                else
                {
                    await _personService.CreateIfNotExist(member.UserOid);
                }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task RemoveAccess(string userOid, string plantId)
        {
            Person? person = await _personService.FindByOid(userOid);

            if (person == null)
            {
                _logger.LogInformation(Resources.PersonDoesNotExist);
                return;
            }
            _logger.LogInformation(string.Format(
                CultureInfo.InvariantCulture,
                Resources.RemoveAccess, person.Id, plantId));
            _personProjectService.RemoveAccessToPlant(person.Id, plantId);
        }

        private async Task GiveAccess(string userOid, string plantId)
        {
            Person? person = await _personService.FindByOid(userOid);

            if (person == null)
            {
                _logger.LogError(Resources.PersonWasNotFoundOrCreated, userOid);
                return;
            }
            _logger.LogInformation(Resources.AddAccess, person.Id, plantId);
            await _personProjectService.GiveProjectAccessToPlant(person.Id, plantId);
        }

        private static bool MessageHasNoRelevantData(AccessInfo accessInfo)
            => accessInfo.Members == null;
    }
}