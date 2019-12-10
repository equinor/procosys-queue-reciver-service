using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
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

        public async void FindAndUpdate(string memberOid)
        {
            var aadPerson = await _graphService.GetPersonByOid(memberOid);

            if (aadPerson.MobileNumber == null || aadPerson.GivenName == null || aadPerson.Surname == null)
                return;

            Person? person = await _personRepository.FindByNameAndMobileNumber(
                                                                aadPerson.MobileNumber,
                                                                aadPerson.GivenName,
                                                                aadPerson.Surname);
            if(person != null)
            {
                person.Oid = memberOid;
                _personRepository.Update(person);
                await _personRepository.SaveChangesAsync();

            }
        }

        public async Task<Person?> FindByOid(string userOid)
        {
            var person = await _personRepository.FindByUserOid(userOid);

            //if (person != null)
            //{
            //    return person;
            //}
            //var adPerson = await _graphService.GetPersonByOid(userOid);
            //person = await FindUserByEmailOrUserName(adPerson);

            //if (person != null)
            //{
            //    person.Oid = adPerson.Oid;
            //    _personRepository.Update(person);
            //    await _personRepository.SaveChangesAsync();
            //}
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

            /**
             * The section checking if the user already exists can be removed once the 
             * database is fully migrated and all users have an OID
             **/
            person = await FindByFullNameAndPhoneNumber(adPerson);
            if (person != null)
            {
                return person;
            }


            person = await _personRepository.AddPerson(
                                new Person(adPerson.Username, adPerson.Email)
                                {
                                    Oid = adPerson.Oid,
                                    FirstName = adPerson.GivenName,
                                    LastName = adPerson.Surname
                                });
            await _personRepository.SaveChangesAsync();
            return person;
        }

        private async Task<Person?> FindByFullNameAndPhoneNumber(AdPerson aadPerson)
            => await _personRepository.FindByNameAndMobileNumber(
                                                                aadPerson.MobileNumber,
                                                                aadPerson.GivenName,
                                                                aadPerson.Surname);
    }
}
