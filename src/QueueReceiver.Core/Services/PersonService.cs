using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
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

        public async Task<Person?> UpdateWithOidIfNotFound(string userOid)
        {
            var person = await _personRepository.FindByUserOid(userOid);

            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetPersonByOid(userOid);

            if (adPerson == null)
            {
                return person; //TODO: This will always be null. Intentionally?
            }

            person = await FindUserByEmailOrUserName(adPerson);

            if (person != null)
            {
                person.Oid = adPerson.Oid;
            }

            return person;
        }

        public async Task<Person> CreateIfNotExist(string userOid)
        {
            var person = await _personRepository.FindByUserOid(userOid);
            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetPersonByOid(userOid);

            if(adPerson == null)
            {
                throw new Exception($"{userOid} not found in graph. Queue out of sync");
            }
            /**
             * The section checking if the user already exists can be removed once the 
             * database is fully migrated and all users have an OID
             **/
            person = await FindUserByEmailOrUserName(adPerson);

            if (person != null)
            {
                person.Oid = userOid;
                return person;
            }
            return await _personRepository.AddPerson(
                    new Person(adPerson.Username, adPerson.Email)
                    {
                        Oid = adPerson.Oid,
                        FirstName = adPerson.GivenName,
                        LastName = adPerson.Surname
                    });
        }

        private async Task<Person> FindUserByEmailOrUserName(AdPerson adPerson)
        {
            var person = await _personRepository.FindByUsername(adPerson.Username)
                         ?? await _personRepository.FindByUserEmail(adPerson.Email);

            return person;
        }

        public async Task<Person?> FindByOid(string userOid) => await _personRepository.FindByUserOid(userOid);
    }
}