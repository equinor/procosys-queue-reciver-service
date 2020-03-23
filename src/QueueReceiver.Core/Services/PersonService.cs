﻿using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace QueueReceiver.Core.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IGraphService _graphService;
        private readonly IProjectRepository _projectRepository;

        public PersonService(IPersonRepository personRepository, 
            IGraphService graphService, 
            IProjectRepository projectRepository)
        {
            _personRepository = personRepository;
            _graphService = graphService;
            _projectRepository = projectRepository;
        }

        public async Task VoidPersonAsync(long personId)
        {
            var person = await _personRepository.FindAsync(personId);
            person.IsVoided = true;
        }

        public async Task UnVoidPersonAsync(long personId)
        {
            var person = await _personRepository.FindAsync(personId);
            person.IsVoided = false;
        }

        public async Task<Person?> UpdateWithOidIfNotFound(string userOid)
        {
            var person = await _personRepository.FindByUserOidAsync(userOid);
            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetAdPersonByOidAsync(userOid);

            return adPerson != null ? await FindAndUpdateAsync(adPerson) : null;
        }

        public async Task<Person?> FindPersonByOidAsync(string userOid) => await _personRepository.FindByUserOidAsync(userOid);

        public async Task<long> GetPersonIdByOidAsync(string userOid) => await _personRepository.FindPersonIdByUserOidAsync(userOid);

        public async Task<Person> CreateIfNotExist(string userOid) //TODO refactor this method, its too big, and does too much.
        {
            var person = await _personRepository.FindByUserOidAsync(userOid);
            if (person != null)
            {
                return person;
            }

            var adPerson = await _graphService.GetAdPersonByOidAsync(userOid);

            if (adPerson == null)
            {
                throw new Exception($"{userOid} not found in graph. Queue out of sync ");
            }
            if (adPerson.MobileNumber != null && adPerson.GivenName != null && adPerson.Surname != null)
            {
                person = await _personRepository.FindByMobileNumberAndNameAsync(
                                                            adPerson.MobileNumber,
                                                            adPerson.GivenName,
                                                            adPerson.Surname);
            }

            if (person?.Oid != null && await _graphService.AdPersonFoundInDeletedDirectory(person.Oid))
            {
                person.Oid = adPerson.Oid;
                return person;
            }

            if (person != null)
            {
                person.Oid = userOid;
                return person;
            }

            if (await ShouldReconcile(adPerson) is IEnumerable<Person> reconcilePersons)
            {
                reconcilePersons.ToList().ForEach(rp=> rp.Reconcile = adPerson.Oid);
                return null;
            }

            return await CreatePerson(adPerson);
        }

        #region SyncServiceMethods
        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
            => _personRepository.GetAllNotInDb(oids);

        public async Task<IEnumerable<string>> GetMembersWithOidAndAccessToPlant(string plantId)
        {
            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);
            var personOids = projects.SelectMany(p => _personRepository.GetOidsBasedOnProject(p.ProjectId)).Distinct();

            return personOids ?? new List<string?>();
        }
        #endregion

        private async Task<Person?> FindAndUpdateAsync(AdPerson adPerson)
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

        private async Task<IEnumerable<Person>?> ShouldReconcile(AdPerson adPerson)
        {
            var possibleMatches = await _personRepository.FindPossibleMatches(adPerson.MobileNumber,adPerson.GivenName,
                adPerson.Surname,adPerson.Username);

            return possibleMatches.Any() ? possibleMatches : null;
        }

        private async Task<Person> CreatePerson(AdPerson adPerson)
        {
            if(adPerson.Username == null )
            {
                return null; //TODO
            }

            var userName = adPerson.Username.ToUpperInvariant();

            return await _personRepository.AddPersonAsync(
                    new Person(userName, adPerson.Email)
                    {
                        Oid = adPerson.Oid,
                        FirstName = adPerson.GivenName,
                        LastName = adPerson.Surname,
                    });
        }
    }
}