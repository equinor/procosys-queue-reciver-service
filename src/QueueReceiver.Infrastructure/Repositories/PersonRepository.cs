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
            mobileNumber = mobileNumber.Replace(" ", "");

            return await _persons.FirstOrDefaultAsync(p =>
                p.MobilePhoneNumber != null
                && MobileNumberIsEqual(mobileNumber, p.MobilePhoneNumber)
                && givenName.Equals(p.FirstName)
                && surname.Equals(p.LastName));
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
                userOid.Equals(person.Oid, OrdinalIgnoreCase));

        public async Task<long> FindPersonIdByUserOidAsync(string userOid)
            => await _persons
                .Where(person => userOid.Equals(person.Oid))
                .Select(person => person.Id)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Person>> FindPossibleMatches(
            string mobileNumber, 
            string firstName, 
            string lastName, 
            string userName)
        {
            var shortName = userName?.Substring(0, userName.IndexOf('@', OrdinalIgnoreCase));

            return await _persons.Where(person =>
                (person.MobilePhoneNumber != null
                 && MobileNumberIsEqual(mobileNumber, person.MobilePhoneNumber))
                || (firstName.Equals(person.FirstName, OrdinalIgnoreCase)
                    && lastName.Equals(person.LastName, OrdinalIgnoreCase))
                || string.Equals(shortName, person.UserName, OrdinalIgnoreCase)
                || string.Equals(userName, person.UserName, OrdinalIgnoreCase)
            ).ToListAsync();
        }

        public IEnumerable<string> GetOidsBasedOnProject(long projectId)
        {
            var persons = _persons.Include(p => p.PersonProjects)
                .Where(p => p.PersonProjects != null
                            && p.PersonProjects.Select(pp => pp.ProjectId).Contains(projectId)
                            && p.PersonProjects.Any(pp => !pp.IsVoided))
                .Distinct()
                .ToList();

            return persons.Where(p => p.Oid != null).Select(p => p.Oid!);
        }

        private static bool MobileNumberIsEqual(string a, string b)
             => a.Equals(b.Replace(" ", "")) || a.Equals("+47" + b.Replace(" ", ""));
    }
}
