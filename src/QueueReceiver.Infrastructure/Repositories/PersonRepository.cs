using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    #pragma warning disable CA1304 // Specify CultureInfo (we don't use ToUpperInvariant or specify culture, as EF cannot translate this to SQL)
    public class PersonRepository : IPersonRepository
    {
        private readonly DbSet<Person> _persons;

        public PersonRepository(QueueReceiverServiceContext context)
        {
            _persons = context.Persons;
        }

        public Task<Person> FindAsync(long personId)
        {
            return _persons.FindAsync(personId);
        }

        public async Task<Person> AddPersonAsync(Person person)
        {
            await _persons.AddAsync(person);
            return person;
        }

        public async Task<Person?> FindByMobileNumberAndNameAsync(string mobileNumber, string givenName, string surname)
        {
            mobileNumber = string.IsNullOrEmpty(mobileNumber) ? string.Empty : mobileNumber.Replace(" ", string.Empty);

            return await _persons.FirstOrDefaultAsync(p =>
                p.FirstName != null &&
                p.LastName != null &&
                p.MobilePhoneNumber != null &&
                p.FirstName.ToUpper().Equals(givenName.ToUpper()) &&
                p.LastName.ToUpper().Equals(surname.ToUpper()) &&
                (mobileNumber.Equals(p.MobilePhoneNumber) ||
                 mobileNumber.Equals("+47" + p.MobilePhoneNumber)));
        }

        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
        {
            var withOid = _persons
                .Where(p => p.Oid != null)
                .Select(p => p.Oid!)
                .AsNoTracking()
                .ToAsyncEnumerable();

            return oids.ToAsyncEnumerable().Except(withOid).ToEnumerable();
        }

        public async Task<Person?> FindByUserOidAsync(string userOid)
            => await _persons.FirstOrDefaultAsync(person =>
                userOid.Equals(person.Oid));

        public async Task<long> FindPersonIdByUserOidAsync(string userOid)
            => await _persons
                .Where(person => userOid.Equals(person.Oid))
                .Select(person => person.Id)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Person>> FindPossibleMatches(
            string mobileNumber, 
            string firstName, 
            string lastName, 
            string userName,
            string email)
        {
            userName = userName.ToUpper();

            var shortName = string.IsNullOrEmpty(userName) || !userName.Contains("@")
                ? string.Empty
                : userName.Substring(0, userName.IndexOf('@')).ToUpper();

            mobileNumber = string.IsNullOrEmpty(mobileNumber) ? string.Empty : mobileNumber.Replace(" ", string.Empty);
            firstName = string.IsNullOrEmpty(firstName) ? string.Empty : firstName.ToUpper();
            lastName = string.IsNullOrEmpty(lastName) ? string.Empty : lastName.ToUpper();
            email = string.IsNullOrEmpty(email) ? string.Empty : email.ToUpper();

            return await _persons.Where(person =>
                    (person.MobilePhoneNumber != null &&
                     mobileNumber.Equals(person.MobilePhoneNumber))
                    || (person.MobilePhoneNumber != null &&
                        mobileNumber.Equals("+47" + person.MobilePhoneNumber))
                    || (email.Equals(person.Email.ToUpper()))
                    || (person.FirstName != null &&
                        person.LastName != null &&
                        firstName.Equals(person.FirstName.ToUpper()) &&
                        lastName.Equals(person.LastName.ToUpper()))
                    || string.Equals(shortName, person.UserName.ToUpper())
                    || string.Equals(userName, person.UserName.ToUpper()))
                .ToListAsync();
        }

        public IEnumerable<string> GetOidsBasedOnProject(long projectId)
        {
            return _persons
                .Include(p => p.PersonProjects)
                .Where(p => p.Oid != null &&
                            p.PersonProjects.Any(pp => pp.ProjectId == projectId && !pp.IsVoided))
                .Select(p => p.Oid!)
                .Distinct();
        }
    }
}
