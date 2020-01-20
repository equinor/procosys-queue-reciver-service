using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Threading.Tasks;
using static System.StringComparison;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Person> _persons;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
            _persons = context.Persons;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _persons.AddAsync(person);
            return person;
        }

        public Task<Person> FindByUserEmail(string userEmail) =>
            _persons.SingleOrDefaultAsync(person =>
                userEmail.Equals(person.Email, OrdinalIgnoreCase));

        public Task<Person> FindByUsername(string userName)
        {
           var shortName = userName.Substring(0, userName.IndexOf('@', OrdinalIgnoreCase));
           return _persons
               .SingleOrDefaultAsync(person =>
                    userName.Equals(person.UserName, OrdinalIgnoreCase)
                    || shortName.Equals(person.UserName, OrdinalIgnoreCase)
                    );
        }

        public Task<Person> FindByUserOid(string userOid) =>
            _persons.SingleOrDefaultAsync(person => 
                userOid.Equals(person.Oid, OrdinalIgnoreCase));

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Update(Person person)
            => _context.Update(person);
    }
}
