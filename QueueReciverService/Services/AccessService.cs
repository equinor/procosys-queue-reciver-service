using System.Threading.Tasks;
using QueueReciverService.Models;
using QueueReciverService.Repositories;
using Microsoft.Extensions.Logging;

namespace QueueReciverService.Services
{
    public class AccessService : IAccessService
    {
        private readonly IPersonRepository _personRepository;

        public ILogger<AccessService> _logger { get; }

        public AccessService(IPersonRepository personRepository, ILogger<AccessService> logger)
        {
            _personRepository = personRepository;
            _logger = logger;
        }

        public async Task<bool> HandleRequest(AccessInfo accessInfo)
        {
            Person person = await _personRepository.FindByUserOid(accessInfo.UserOid) ??
               await FindByEmailOrUserNameAndUpdateOid(accessInfo);

            if (person == null && !accessInfo.HasAccess)
            {
                _logger.LogInformation("Trying to remove access from person that doesn't exist");
                return await Task.FromResult(true);
            }

            if (person == null && accessInfo.HasAccess)
            {
                _logger.LogInformation($"Person with oid {accessInfo.UserOid} not found in database, creating person");

                await _personRepository.AddPerson(new Person
                {
                    Oid = accessInfo.UserOid,
                    Email = accessInfo.UserEmail,
                    UserName = accessInfo.UserName
                });
                await _personRepository.SaveChangesAsync();

            }
            
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

            var person =  await _personRepository.FindByUserEmail(accessInfo.UserEmail)
                ?? await _personRepository.FindByUserName(accessInfo.UserName);

            if(person != null)
            {
                _logger.LogInformation($"Adding Oid {accessInfo.UserOid} to user with id = {person.Id}");
                person.Oid = accessInfo.UserOid;
                _personRepository.Update(person);
                await _personRepository.SaveChangesAsync();
            }
            return person;
        }

        public Task<bool> RemoveAccess(long personId, string plantId)
        {
            throw new System.NotImplementedException(); 
        }

        public Task<bool> GiveAccess(long personId, string plantId)
        {
            throw new System.NotImplementedException();
        }

    }
}
