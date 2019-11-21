using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
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

            accessInfo.Members.AsParallel().ForAll(async member =>
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
        }

        private async Task RemoveAccess(Member member, string plantId)
        {
            Person person = await _personService.FindOrCreate(member.UserOid, shouldRemove: true);
            _logger.LogInformation($"Removing access for person with id: {person.Id}, to plant {plantId}");

            if(person == null)
            {
                return;
            }

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

