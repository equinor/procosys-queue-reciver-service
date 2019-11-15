using QueueReciverService.Models;
using QueueReciverService.Repositories;
using System.Threading.Tasks;

namespace QueueReciverService.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IGraphService _graphService;
        public PersonService(IPersonRepository personRepository, IGraphService graphService)
        {
            _personRepository = personRepository;
            _graphService = graphService;
        }

        

        public async Task<Person> FindOrCreate(string userOid, bool shouldCreate)
        {
            Person person = await _personRepository.FindByUserOid(userOid);

            if (person == null)
            {
                Person graphPerson = await _graphService.GetPersonByOid(userOid);
                person = await _personRepository.FindByUserEmail(graphPerson.Email)
                   ?? await _personRepository.FindByUsername(graphPerson.UserName);

                if (person == null && shouldCreate)
                {
                    person = await _personRepository.AddPerson(graphPerson);
                }
                else if(person == null)
                {
                    return null;
                }
                else
                {
                    person.Oid = userOid;
                    await _personRepository.Update(person);
                }
                var success = await _personRepository.SaveChangesAsync();

                if (!success)
                {
                   // _logger.LogError($"Unable to add person with oid: {member.UserOid} to db");
                   //todo throw exception/complex return?
                    return null;
                }

            }
            return person;
        }

    }
}
