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

        public async ValueTask<bool> HandleRequest(AccessInfo accessInfo)
        {
            var groupExistsInDb = await _plantService.Exists(accessInfo.PlantOid);

            if (!groupExistsInDb)
            {
                _logger.LogInformation($"Group not relevant, returning success to remove message from queue");
                return true;
            }

            if(accessInfo.Members == null)
            {
                //irrelevant
                return true;
            }

            var resultTasks = accessInfo.Members.Select(async member =>
            {
                if (member.ShouldRemove)
                {
                    return await RemoveAccess(member, accessInfo.PlantOid);
                }
                return await GiveAccess(member, accessInfo.PlantOid);
            });

            bool[]  results = await Task.WhenAll(resultTasks);
            return results.Aggregate((a, b) => a && b);
        }

        private async ValueTask<bool> RemoveAccess(Member member, string plantOid)
        {
            var (person, success) = await _personService.FindOrCreate(member.UserOid, shouldNotCreate: true);

            if (!success)
            {
                return false;
            }

            if (person == null)
            {
                _logger.LogInformation($"Person with oid: {member.UserOid}, not found in database, no access to remove.");
                return success;
            }

            _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {plantOid}");
            return await _projectService.RemoveAccessToPlant(person.Oid, plantOid);
        }

       private async ValueTask<bool> GiveAccess(Member member, string plantOid) 
       {
            var (person, success) = await _personService.FindOrCreate(member.UserOid);

            if (!success)
            {

                return false;
            }

            _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {plantOid}");
            return await _projectService.GiveAccessToPlant(person.Oid, plantOid);
       }
    }
}

