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

        public async Task<Person?> UpdateWithOidIfNotFoundAsync(string userOid)
        {
            var person = await _personRepository.FindByUserOidAsync(userOid);
            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetAdPersonByOidAsync(userOid);

            return adPerson != null ? await FindAndUpdateAsync(adPerson) : null;
        }

        public async Task<Person?> FindAndUpdateAsync(AdPerson adPerson)
        {
            if (adPerson.MobileNumber == null || adPerson.GivenName == null || adPerson.Surname == null)
            {
                return null;
            }

            var person = await _personRepository.FindByMobileNumberAndNameAsync(
                                                                adPerson.MobileNumber,
                                                                adPerson.GivenName,
                                                                adPerson.Surname);
            if (person != null)
            {
                person.Oid = adPerson.Oid;
            }
            return person;
        }

        public async Task<Person?> FindByOidAsync(string userOid) => await _personRepository.FindByUserOidAsync(userOid);

        public async Task<Person> CreateIfNotExistAsync(string userOid)
        {
            var person = await _personRepository.FindByUserOidAsync(userOid);
            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetAdPersonByOidAsync(userOid);

            if (adPerson == null)
            {
                throw new Exception($"{userOid} not found in graph. Queue out of sync");
            }

            person = await _personRepository.FindByMobileNumberAndNameAsync(
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
            return await _personRepository.FindByFullNameAsync(adPerson.GivenName, adPerson.Surname) != null
                || await _personRepository.FindByMobileNumberAsync(adPerson.MobileNumber) != null
                || await _personRepository.FindByUsernameAsync(adPerson.Username) != null;
        }

        private async Task<Person> CreatePerson(AdPerson adPerson, bool shouldReconcile)
        {
            return await _personRepository.AddPersonAsync(
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