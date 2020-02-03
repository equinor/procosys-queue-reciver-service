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

            return adPerson != null ? await FindAndUpdate(adPerson) : null;
        }

        public async Task<Person?> FindAndUpdate(AdPerson aadPerson)
        {
            if (aadPerson.MobileNumber == null || aadPerson.GivenName == null || aadPerson.Surname == null)
            {
                return null;
            }

            var person = await _personRepository.FindByMobileNumberAndName(
                                                                aadPerson.MobileNumber,
                                                                aadPerson.GivenName,
                                                                aadPerson.Surname);
            if (person != null)
            {
                person.Oid = aadPerson.Oid;
            }
            return person;
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

            person = await _personRepository.FindByMobileNumberAndName(
                                                            adPerson.MobileNumber,
                                                            adPerson.GivenName,
                                                            adPerson.Surname);

            if (person?.Oid != null)
            {
                return await CreatePerson(adPerson, shouldReconcile: true);
            }

            if (person != null)
            {
                person.Oid = userOid;
                return person;
            }

            var shouldReconcile = await ShouldReconcile(adPerson);
            return await CreatePerson(adPerson,shouldReconcile);
        }

        private async Task<bool> ShouldReconcile(AdPerson adPerson)
        {
            return await _personRepository.FindByFullName(adPerson.GivenName, adPerson.Surname) != null
                || await _personRepository.FindByMobileNumber(adPerson.MobileNumber) != null
                || await _personRepository.FindByUsername(adPerson.Username) != null;
        }

        private async Task<Person> CreatePerson(AdPerson adPerson, bool shouldReconcile)
        {
            return await _personRepository.AddPerson(
                    new Person(adPerson.Username, adPerson.Email)
                    {
                        Oid = adPerson.Oid,
                        FirstName = adPerson.GivenName,
                        LastName = adPerson.Surname,
                        Reconcile = shouldReconcile
                    });
        }

        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
            => _personRepository.GetAllNotInDb(oids);
    }
}