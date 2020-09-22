using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Constants;

namespace QueueReceiver.Core.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IGraphService _graphService;
        private readonly IProjectRepository _projectRepository;
        private readonly PersonCreatedByCache _personCreatedByCache;
        private readonly ILogger<PersonService> _logger;
        private readonly IPersonProjectRepository _personProjectRepository;

        public PersonService(
            IPersonRepository personRepository,
            IGraphService graphService,
            IProjectRepository projectRepository,
            PersonCreatedByCache personCreatedByCache,
            IPersonProjectRepository personProjectRepository,
            ILogger<PersonService> logger)
        {
            _personRepository = personRepository;
            _graphService = graphService;
            _projectRepository = projectRepository;
            _personCreatedByCache = personCreatedByCache;
            _personProjectRepository = personProjectRepository;
            _logger = logger;
        }

        public async Task SetPersonCreatedByCache()
        {
            // Set Username from DB (Id is set from AppSettings at startup)
            if (_personCreatedByCache.Username == null && _personCreatedByCache.Id > 0)
            {
                var createdByPerson = await _personRepository.FindAsync(_personCreatedByCache.Id);
                _personCreatedByCache.Username = createdByPerson.UserName;
            }
        }

        public async Task UpdateVoidedStatus(string personOid)
        {
            var personId = await _personRepository.FindPersonIdByUserOidAsync(personOid);

            if(personId == 0)
            {
                return;
            }

            if (await _personProjectRepository.PersonHasNoAccess(personId))
            {
                await VoidPersonAsync(personId);
            }
            else
            {
                await UnVoidPersonAsync(personId);
            }
        }

        private async Task VoidPersonAsync(long personId)
        {
            var person = await _personRepository.FindAsync(personId);
            person.IsVoided = true;
        }

        private async Task UnVoidPersonAsync(long personId)
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

        public async Task CreateIfNotExist(string userOid, string plantId)
        {
            var person = await _personRepository.FindByUserOidAsync(userOid);

            if (person != null)
            {
                return;
            }

            var adPerson = await _graphService.GetAdPersonByOidAsync(userOid);

            if (adPerson == null)
            {
                throw new Exception($"{userOid} not found in graph. Queue out of sync ");
            }

            if (adPerson.MobileNumber != null
                && ((adPerson.GivenName != null && adPerson.Surname != null)
                    || adPerson.DisplayName != null))
            {
                var (firstName, lastName) = GetAdPersonFirstAndLastName(adPerson);

                person = await _personRepository.FindByMobileNumberAndNameAsync(
                    adPerson.MobileNumber,
                    firstName,
                    lastName);
            }

            if (person != null)
            {
                person.Oid = userOid;
                return;
            }

            var reconcilePersons = await GetReconcilePersons(adPerson);

            if (reconcilePersons.Count > 0)
            {
                _logger.LogInformation($"Reconcile: setting OID {userOid} on {reconcilePersons.Count} possible matches.");

                reconcilePersons.ForEach(rp =>
                {
                    _logger.LogInformation($"Reconcile: setting OID {userOid} on person id: {rp.Id}");
                    rp.Reconcile = userOid;
                    rp.ReconcileProjectschema = plantId;
                });

                return;
            }

            await CreatePerson(adPerson);
        }

        #region SyncServiceMethods
        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
            => _personRepository.GetAllNotInDb(oids);

        public async Task<IEnumerable<string>> GetMembersWithOidAndAccessToPlant(string plantId)
        {
            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);
            var personOids = projects.SelectMany(p => _personRepository.GetOidsBasedOnProject(p.ProjectId)).Distinct();

            return personOids;
        }
        #endregion

        private async Task<Person?> FindAndUpdateAsync(AdPerson adPerson)
        {
            if (adPerson.MobileNumber == null ||
                ((adPerson.GivenName == null && adPerson.Surname == null) || adPerson.DisplayName == null))
            {
                return null;
            }

            var (firstName, lastName) = GetAdPersonFirstAndLastName(adPerson);

            var person = await _personRepository.FindByMobileNumberAndNameAsync(
                adPerson.MobileNumber,
                firstName,
                lastName);

            if (person != null)
            {
                person.Oid = adPerson.Oid;
            }

            return person;
        }

        private async Task<List<Person>> GetReconcilePersons(AdPerson adPerson)
        {
            var (firstName, lastName) = GetAdPersonFirstAndLastName(adPerson);

            var possibleMatches = await _personRepository.FindPossibleMatches(
                adPerson.MobileNumber,
                firstName,
                lastName,
                adPerson.Username);

            var reconcilePersons = new List<Person>();
            var adPersonEmailDomain = GetEmailAddressDomain(adPerson.Username);

            // In order to set reconcile, the existing and new users e-mail domains must match.
            // Otherwise, it is most likely a new affiliate user and should not be reconciled.
            foreach (var person in possibleMatches.ToList())
            {
                var reconcilePersonEmail = person.UserName.Contains("@") ? person.UserName : person.Email;
                var reconcilePersonEmailDomain = GetEmailAddressDomain(reconcilePersonEmail);

                if (adPersonEmailDomain == reconcilePersonEmailDomain)
                {
                    reconcilePersons.Add(person);
                }
            }

            return reconcilePersons;
        }

        private async Task CreatePerson(AdPerson adPerson)
        {
            if (adPerson.Username == null)
            {
                _logger.LogError($"AD person with OID: {adPerson.Oid} does not contain a username");
                return;
            }
            
            _logger.LogInformation($"Creating person with OID: {adPerson.Oid}");
            
            var userName = adPerson.Username.ToUpperInvariant();
            var (firstName, lastName) = GetAdPersonFirstAndLastName(adPerson);

            await _personRepository.AddPersonAsync(
                new Person(userName, adPerson.Email)
                {
                    Oid = adPerson.Oid,
                    FirstName = firstName,
                    LastName = lastName,
                    MobilePhoneNumber = adPerson.MobileNumber?.Replace(" ", string.Empty),
                    UpdatedAt = DateTime.Now,
                    CreatedById = _personCreatedByCache.Id,
                    UpdatedById = _personCreatedByCache.Id
                });
        }

        public (string firstName, string lastName) GetAdPersonFirstAndLastName(AdPerson adPerson)
        {
            if (!string.IsNullOrEmpty(adPerson.GivenName) && !string.IsNullOrEmpty(adPerson.Surname))
            {
                return (adPerson.GivenName, adPerson.Surname);
            }

            if (!string.IsNullOrEmpty(adPerson.DisplayName) && adPerson.DisplayName.Contains(" "))
            {
                if (adPerson.DisplayName.Contains(","))
                {
                    // A naming error in Azure sometimes cause DisplayName to be formatted "Lastname, Firstname".
                    // The following is a best effort fix to put Lastname last and remove the comma.
                    var nameToFix = adPerson.DisplayName;
                    var assumedLastName = nameToFix.Substring(0, nameToFix.IndexOf(",", StringComparison.InvariantCulture));
                    nameToFix = nameToFix.Remove(0, assumedLastName.Length);
                    nameToFix = nameToFix.Replace(",", string.Empty);
                    nameToFix = nameToFix.TrimStart();
                    
                    adPerson.DisplayName = $"{nameToFix} {assumedLastName}";
                }

                // last name will be set to the last part of the name, regardless of any middle-name variants (best effort)
                var indexOfLastSpace = adPerson.DisplayName.LastIndexOf(" ", StringComparison.InvariantCulture);
                var firstName = adPerson.DisplayName.Substring(0, indexOfLastSpace);
                var lastName = adPerson.DisplayName.Substring(indexOfLastSpace + 1);

                return (firstName, lastName);
            }

            throw new Exception($"Could not determine first or last name for user with Oid: {adPerson.Oid}");
        }

        public string GetEmailAddressDomain(string email) =>
            !email.Contains("@")
                ? ReconcileConstants.DefaultEmailDomain
                : email.Substring(email.IndexOf('@') + 1).ToUpperInvariant();
    }
}