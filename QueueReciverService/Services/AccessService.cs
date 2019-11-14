using System.Threading.Tasks;
using QueueReciverService.Models;
using QueueReciverService.Repositories;
using Microsoft.Extensions.Logging;

namespace QueueReciverService.Services
{
    public class AccessService : IAccessService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IProjectService _projectService;
        private readonly ILogger<AccessService> _logger;

        public AccessService(
            IPersonRepository personRepository,
            IProjectService projectService,
            ILogger<AccessService> logger)
        {
            _personRepository = personRepository;
            _projectService = projectService;
            _logger = logger;
        }

        public async Task<bool> HandleRequest(AccessInfo accessInfo)
        {
            //TODO Possibly do some mapping from group to plant
            //groupId, userID, remove/add





            Person person = await _personRepository.FindByUserOid(accessInfo.UserOid) ??
               await FindByEmailOrUserNameAndUpdateOid(accessInfo);

            if (person == null && accessInfo.ShouldRemove)
            {
                _logger.LogInformation("Trying to remove access from person that doesn't exist");
                return await Task.FromResult(true);
            }

            if (person == null && !accessInfo.ShouldRemove)
            {
                _logger.LogInformation($"Person with oid {accessInfo.UserOid} not found in database, creating person");

                // var person = await _graphService.GetUserInfo(accessInfo.UserOid)

                person = await _personRepository.AddPerson(person);
                
                var success =  await _personRepository.SaveChangesAsync();

                if (!success)
                {
                    _logger.LogError($"Unable to add person with oid: {accessInfo.UserOid} to db");
                    return false;
                }
            }

            if (accessInfo.ShouldRemove)
            {
                _logger.LogInformation($"Adding access for person with id: {person.Id}, to plant {accessInfo.PlantOid}");
                _projectService.GiveAccessToPlant(person.Id, accessInfo.PlantOid);
            }

            _logger.LogInformation($"Removing access for person with id: {person.Id}, to plant {accessInfo.PlantOid}");
            _projectService.RemoveAccessToPlant(person.Id, accessInfo.PlantOid);




            //Check for give or remove access

            //Give access

            //find person by oid
            //if no person with that oid, check for person via email/shortname 
            //if found by email/shortname, add oid to person

            //if person not found by oid or email/shortname, insert person

            //update person_projects, give access to all projects for given plant

            //Remove access
            //find person by oid

            //if not found, check by email/shortname?? -*uncertain*-
            //if person doesn't exist, do nothing(return success?)

            //find all person projects for given plant
            //Update person_projects, remove all for given plant.

            //return success

            throw new System.NotImplementedException();
        }

        private async Task<Person> FindByEmailOrUserNameAndUpdateOid(AccessInfo accessInfo)
        {
            //TODO call graph api with oid and find username and email.

            var person =  await _personRepository.FindByUserEmail("email")
                ?? await _personRepository.FindByUserName("userName");

            if(person != null)
            {
                _logger.LogInformation($"Adding Oid {accessInfo.UserOid} to user with id = {person.Id}");
                person.Oid = accessInfo.UserOid;
                _personRepository.Update(person);
                await _personRepository.SaveChangesAsync();
            }
            return person;
        }
    }
}
