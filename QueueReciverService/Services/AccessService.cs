using System.Threading.Tasks;
using QueueReciverService.Models;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace QueueReciverService.Services
{
    public class AccessService : IAccessService
    {
        private readonly IPersonService _personService;
        private readonly IProjectService _projectService;
        private readonly IGraphService _graphService;
        private readonly ILogger<AccessService> _logger;

        public AccessService(
            IPersonService personService,
            IProjectService projectService,
            IGraphService graphService,
            ILogger<AccessService> logger)
        {
            _personService = personService;
            _projectService = projectService;
            _graphService = graphService;
            _logger = logger;
        }

        public async ValueTask<bool> HandleRequest(AccessInfo accessInfo)
        {
            var asyncResult = accessInfo.Members.Select(async member => member.ShouldRemove
                 ? await HandleRemoveAccess(member, accessInfo.PlantOid)
                 : await HandleGiveAccess(member, accessInfo.PlantOid));

            var result = await Task.WhenAll(asyncResult);
            return result.Aggregate((a, b) => a && b);
        }

        private async ValueTask<bool> HandleRemoveAccess(Member member, string plantOid)
        {

            var person = await _personService.FindOrCreate(member.UserOid, false);

            if (person == null)
            {
                _logger.LogInformation($"Person with oid: {member.UserOid}, not found in database, no access to remove.");
                return true;
            }

            _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {plantOid}");
            return await _projectService.GiveAccessToPlant(person.Oid, plantOid);

        }
    

       private async ValueTask<bool> HandleGiveAccess(Member member, string plantOid) {

            var person = await _personService.FindOrCreate(member.UserOid, true);

            if (person == null)
            {
                _logger.LogInformation($"Something went wrong,not able to find or create person with oid {member.UserOid}");
                return false;
            }
                _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {plantOid}");
               return await _projectService.GiveAccessToPlant(person.Oid, plantOid);

        }
    }
}

