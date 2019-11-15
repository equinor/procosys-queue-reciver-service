using System.Threading.Tasks;
using QueueReciverService.Models;
using Microsoft.Extensions.Logging;

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






            Person person = await _personService.FindByOid(accessInfo.UserOid);
            
            //TODO too many ifs else, refactor

            if(person == null)
            {
                Person graphPerson = await _graphService.GetPersonByOid(accessInfo.UserOid);

                 person = await _personService.FindByEmail(graphPerson.Email) 
                    ?? await _personService.FindByUsername(graphPerson.UserName);

                
                if (person == null)
                {
                    //TODO log
                    if (accessInfo.ShouldRemove)
                    {
                        _logger.LogInformation("Trying to remove access from person that doesn't exist in the db");
                        //*need to return true so message gets removed from queue*//
                        return true; 
                    }

                    _logger.LogInformation($"Person with oid {accessInfo.UserOid} not found in database, creating person");
                    person = await _personService.Add(graphPerson);
                }
                else
                {
                    person.Oid = accessInfo.UserOid;
                    await  _personService.Update(person);
                }

                var success = await _personService.SaveChangesAsync();

                if (!success)
                {
                    _logger.LogError($"Unable to add person with oid: {accessInfo.UserOid} to db");
                    return false;
                }
            }

            if (accessInfo.ShouldRemove)
            {
                _logger.LogInformation($"Removing access for person with id: {person.Id}, to plant {accessInfo.PlantOid}");
               return await _projectService.RemoveAccessToPlant(person.Oid, accessInfo.PlantOid);
            }
          
                _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {accessInfo.PlantOid}");
               return await _projectService.GiveAccessToPlant(person.Oid, accessInfo.PlantOid);

        }
    }
}
