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

        public async ValueTask<(Person person, bool success)> FindOrCreate(string userOid, bool shouldCreate = true)
        {
            var person = await _personRepository.FindByUserOid(userOid);

            if (person != null)
            {
                return (person, success: true);
            }

            var adPerson = await _graphService.GetPersonByOid(userOid);

            person = await _personRepository.FindByUserEmail(adPerson.Email)
                     ?? await _personRepository.FindByUsername(adPerson.UserName);

            switch (person)
            {
                case null when !shouldCreate:
                    return (person: null, success: true);
                case null:
                    person = await _personRepository.AddPerson(
                    new Person
                    {
                        Oid = adPerson.Oid,
                        Email = adPerson.Email,
                        UserName = adPerson.UserName
                    });
                    break;
                default:
                    person.Oid = userOid;
                    _personRepository.Update(person);
                    break;
            }

            var success = await _personRepository.SaveChangesAsync();

            return (person, success);
        }

    }
}
