using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using QueueReceiver.Core.Properties;
using System.Globalization;

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

            if (plantId == null)
            {
                _logger.LogInformation(Resources.GroupDoesNotExist);
                return;
            }

            if (MessageHasNoRelevantData(accessInfo))
            {
                return;
            }

          var syncPersonTableTasks = accessInfo.Members.Select(async member =>
            {
                if (!member.ShouldRemove)
                {
                   await _personService.CreateIfNotExist(member.UserOid);
                }
                else
                {
                    await _personService.FindOrUpdate(member.UserOid);
                }
            });
            await Task.WhenAll(syncPersonTableTasks);
            await _unitOfWork.SaveChangesAsync();

            var syncAccessTasks = accessInfo.Members.Select(async member =>
            {
                if (member.ShouldRemove)
                {
                    await RemoveAccess(member, plantId);
                }
                else
                {
                    await GiveAccess(member, plantId);
                }
            });

            await Task.WhenAll(syncAccessTasks);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task RemoveAccess(Member member, string plantId)
        {
            Person? person = await _personService.FindByOid(member.UserOid);

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

        private async Task GiveAccess(Member member, string plantId)
        {
            Person? person = await _personService.FindByOid(member.UserOid);

            if (person == null)
            {
                _logger.LogError(Resources.PersonWasNotFoundOrCreated,member.UserOid);
                return;
            }
            _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {plantId}");
            await _personProjectService.GiveProjectAccessToPlant(person.Id, plantId);
        }

        private static bool MessageHasNoRelevantData(AccessInfo accessInfo)
            => accessInfo.Members == null;
    }
}