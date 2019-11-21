using System.Threading.Tasks;
using QueueReceiverService.Models;
using QueueReceiverService.Repositories;

namespace QueueReceiverService.Services
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

        public async Task<Person> FindOrCreate(string userOid, bool shouldNotCreate = false)
        {
            var person = await _personRepository.FindByUserOid(userOid);

            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetPersonByOid(userOid);

            person = await _personRepository.FindByUserEmail(adPerson.Email)
                     ?? await _personRepository.FindByUsername(adPerson.Username);

            switch (person)
            {
                case null when shouldNotCreate:
                    return null;
                case null:
                    person = await _personRepository.AddPerson(
                    new Person
                    {
                        Oid = adPerson.Oid,
                        Email = adPerson.Email,
                        UserName = adPerson.Username
                    });
                    break;
                default:
                    person.Oid = userOid;
                    _personRepository.Update(person);
                    break;
            }
            await _personRepository.SaveChangesAsync();
            return person;
        }

    }
}
