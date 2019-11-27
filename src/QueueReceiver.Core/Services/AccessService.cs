using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class AccessService : IAccessService
    {
        private readonly IPersonService _personService;
        private readonly IProjectService _projectService;
        private readonly IPlantService _plantService;
        private readonly ILogger<AccessService> _logger;

        public AccessService(
            IPersonService personService,
            IProjectService projectService,
            IPlantService plantService,
            ILogger<AccessService> logger)
        {
            _personService = personService;
            _projectService = projectService;
            _plantService = plantService;
            _logger = logger;
        }

        public async Task HandleRequest(AccessInfo accessInfo)
        {
            string plantId = await _plantService.GetPlantId(accessInfo.PlantOid);

            if (plantId == null)
            {
                _logger.LogInformation($"Group not relevant, removing message from queue");
                return;
            }

            if (MessageHasNoRelevantData(accessInfo))
            {
                return;
            }

           var runningJobs = accessInfo.Members.AsParallel().Select(async member =>
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

            await Task.WhenAll(runningJobs);
        }

        private async Task RemoveAccess(Member member, string plantId)
        {
            Person? person = await _personService.FindByOid(member.UserOid);

            if (person == null)
            {
                _logger.LogInformation($"Person doesn't exist in db so there is no reson to remove access," +
                    " removing message from queue");
                return;
            }
            _logger.LogInformation($"Removing access for person with id: {person.Id}, to plant {plantId}");
            await _projectService.RemoveAccessToPlant(person.Id, plantId);
        }

        private async Task GiveAccess(Member member, string plantId)
        {
            Person person = await _personService.FindOrCreate(member.UserOid);

            _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {plantId}");
            await _projectService.GiveProjectAccessToPlant(person.Id, plantId);
        }

        private static bool MessageHasNoRelevantData(AccessInfo accessInfo)
        {
            return accessInfo.Members == null;
        }
    }
}

