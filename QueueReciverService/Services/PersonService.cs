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

        public async ValueTask<(Person,bool)> FindOrCreate(string userOid, bool shouldCreate)
        {
            var person = await _personRepository.FindByUserOid(userOid);

            if (person != null)
            {
                return (person,true);
            }

            var graphPerson = await _graphService.GetPersonByOid(userOid);

            person = await _personRepository.FindByUserEmail(graphPerson.Email)
                     ?? await _personRepository.FindByUsername(graphPerson.UserName);

            switch (person)
            {
                case null when !shouldCreate:
                    return (null, false);
                case null:
                    person = await _personRepository.AddPerson(graphPerson);
                    break;
                default:
                    person.Oid = userOid;
                    await _personRepository.Update(person);
                    break;
            }

            var success = await _personRepository.SaveChangesAsync();

            return (person,success);
        }

    }
}
