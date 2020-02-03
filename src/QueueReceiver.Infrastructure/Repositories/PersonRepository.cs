using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.StringComparison;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DbSet<Person> _persons;

        public PersonRepository(QueueReceiverServiceContext context)
        {
            _persons = context.Persons;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _persons.AddAsync(person);
            return person;
        }

        public async Task<Person?> FindByMobileNumberAndName(string mobileNumber, string givenName, string surname)
        {
            mobileNumber = mobileNumber.Replace(" ", "");

            return await _persons.FirstOrDefaultAsync(p =>
                p.MobilePhoneNumber != null
                && MobileNumberIsEqal(mobileNumber, p.MobilePhoneNumber)
                && givenName.Equals(p.FirstName)
                && surname.Equals(p.LastName));
        }

        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
        {
            var withOid = _persons.Where(p => p.Oid != null).Select(p => p.Oid!).AsNoTracking().ToAsyncEnumerable();
            return oids.ToAsyncEnumerable().Except(withOid).ToEnumerable();
        }

        public async Task<Person?> FindByUserOid(string userOid) =>
           await _persons.FirstOrDefaultAsync(person =>
                userOid.Equals(person.Oid, OrdinalIgnoreCase));

        public async Task<Person?> FindByMobileNumber(string mobileNumber) =>
            await _persons.FirstOrDefaultAsync(p =>
                p.MobilePhoneNumber != null
                && MobileNumberIsEqal(mobileNumber, p.MobilePhoneNumber));

        public async Task<Person?> FindByFullName(string firstName, string lastName) =>
            await _persons.FirstOrDefaultAsync(person =>
                firstName.Equals(person.FirstName, OrdinalIgnoreCase)
                && lastName.Equals(person.LastName,OrdinalIgnoreCase));

        public async Task<Person?> FindByEmail(string userEmail) =>
           await  _persons.FirstOrDefaultAsync(person =>
                    userEmail.Equals(person.Email, OrdinalIgnoreCase));

        public async Task<Person> FindByUsername(string userName)
        {
            var shortName = userName.Substring(0, userName.IndexOf('@', OrdinalIgnoreCase));
            return await _persons
                .SingleOrDefaultAsync(person =>
                     userName.Equals(person.UserName, OrdinalIgnoreCase)
                     || shortName.Equals(person.UserName, OrdinalIgnoreCase)
                     );
        }

        private static bool MobileNumberIsEqal(string a, string b)
             => a.Equals(b.Replace(" ", "")) || a.Equals("+47" + b.Replace(" ", ""));
    }
}


