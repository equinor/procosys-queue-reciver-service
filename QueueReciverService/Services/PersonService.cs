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

        public async Task<Person?> FindByOid(string userOid)
        {
            var person = await _personRepository.FindByUserOid(userOid);

            if (person != null)
            {
                return person;
            }
            var adPerson = await _graphService.GetPersonByOid(userOid);
            person = await FindUseByEmailOrUserName(adPerson);

            if(person != null)
            {
                person.Oid = adPerson.Oid;
                _personRepository.Update(person);
                await _personRepository.SaveChangesAsync();
            }
            return person;
        }

        public async Task<Person> FindOrCreate(string userOid)
        {
            var person = await _personRepository.FindByUserOid(userOid);

            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetPersonByOid(userOid);

            person = await FindUseByEmailOrUserName(adPerson);

            if(person == null)
            {
                person = await _personRepository.AddPerson(
                                    new Person
                                    {
                                        Oid = adPerson.Oid,
                                        Email = adPerson.Email,
                                        UserName = adPerson.Username
                                    });
            }
            await _personRepository.SaveChangesAsync();
            return person;
        }

        private async Task<Person> FindUseByEmailOrUserName(AdPerson adPerson)
        {
            var person = await _personRepository.FindByUserEmail(adPerson.Email)
                      ?? await _personRepository.FindByUsername(adPerson.Username);

            return person;
        }
    }
}
