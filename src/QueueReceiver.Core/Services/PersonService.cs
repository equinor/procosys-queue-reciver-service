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
        private readonly IPersonProjectRepository _personProjectRepository;

        public PersonService(IPersonRepository personRepository, IGraphService graphService, IProjectRepository projectRepository, IPersonProjectRepository personProjectRepository)
        {
            _personRepository = personRepository;
            _graphService = graphService;
            _projectRepository = projectRepository;
            _personProjectRepository = personProjectRepository;
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

        public async Task<Person?> FindPersonByOidAsync(string userOid) => await _personRepository.FindByUserOidAsync(userOid);

        public async Task<long> GetPersonIdByOidAsync(string userOid) => await _personRepository.FindPersonIdByUserOidAsync(userOid);

        public async Task<Person> CreateIfNotExist(string userOid)
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
            if (adPerson.MobileNumber != null && adPerson.GivenName != null && adPerson.Surname != null)
            {
                person = await _personRepository.FindByMobileNumberAndNameAsync(
                                                            adPerson.MobileNumber,
                                                            adPerson.GivenName,
                                                            adPerson.Surname);
            }


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
            if(adPerson.Email == null || adPerson.Oid == null)
            {
                return null; //TODO
            }
             // TODO: What to do if username exists

            var userName = adPerson.Email.ToUpperInvariant();
            return await _personRepository.AddPersonAsync(
                    new Person(userName, adPerson.Email)

                    {
                        Oid = adPerson.Oid,
                        FirstName = adPerson.GivenName,
                        LastName = adPerson.Surname,
                        Reconcile = shouldReconcile
                    });
        }

        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
            => _personRepository.GetAllNotInDb(oids);

        public async Task<IEnumerable<string>> GetMembersWithOidAndAccessToPlant(string plantId)
        {
            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);
            var personOids = projects.SelectMany(p => _personRepository.GetOidsBasedOnProject(p.ProjectId)).Distinct();

            
            if(personOids == null)
            {
                return new List<string>();
            }
            return personOids;
        }
    }
}