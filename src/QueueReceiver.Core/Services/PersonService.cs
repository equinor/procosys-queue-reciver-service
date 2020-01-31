using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Collections.Generic;
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

            if (adPerson == null || adPerson.MobileNumber == null || adPerson.GivenName == null || adPerson.Surname == null)
            {
                return null;
            }

            person = await _personRepository.FindByMobileNumberAndName(
                                                                adPerson.MobileNumber,
                                                                adPerson.GivenName,
                                                                adPerson.Surname);

            //Find by only full name and set reconcile?

            //find by username/email/shortname and set reconcile?

            if(person?.Oid != null)
            {
                //TODO
                //reconsile
                //Create person?

            }
            else if(person != null)
            {
                person.Oid = adPerson.Oid;
            }
            return person;
        }

        public async Task FindAndUpdate(AdPerson aadPerson)
        {
            if (aadPerson.MobileNumber == null || aadPerson.GivenName == null || aadPerson.Surname == null)
            {
                return;
            }

            var person = await _personRepository.FindByMobileNumberAndName(
                                                                aadPerson.MobileNumber,
                                                                aadPerson.GivenName,
                                                                aadPerson.Surname);
            if (person != null)
            {
                person.Oid = aadPerson.Oid;
            }
        }

        public async Task<Person?> FindByOid(string userOid) => await _personRepository.FindByUserOid(userOid);

        public async Task<Person> CreateIfNotExist(string userOid)
        {
            var person = await _personRepository.FindByUserOid(userOid);
            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetPersonByOid(userOid);

            if (adPerson == null)
            {
                throw new Exception($"{userOid} not found in graph. Queue out of sync");
            }
            /**
             * The section checking if the user already exists can be removed once the 
             * database is fully migrated and all users have an OID
             **/
            person = await _personRepository.FindByMobileNumberAndName(adPerson.MobileNumber, adPerson.GivenName, adPerson.Surname);
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

        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
            => _personRepository.GetAllNotInDb(oids);
    }
}