using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Person> _persons;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
            _persons = _context.Persons;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _persons.AddAsync(person);
            return person;
        }

        public Task<Person> FindByUserEmail(string userEmail)
        {
            return _persons
                 .SingleOrDefaultAsync(person => 
                    userEmail.Equals(person.Email, System.StringComparison.OrdinalIgnoreCase));
        }

        public Task<Person> FindByUsername(string userName)
        {
           var shortname = userName.Substring(0, userName.IndexOf('@'));
            return _persons
                .SingleOrDefaultAsync(person =>
                    userName.Equals(person.UserName, System.StringComparison.OrdinalIgnoreCase)
                    || shortname.Equals(person.UserName, System.StringComparison.OrdinalIgnoreCase)
                    );
        }

        public Task<Person> FindByUserOid(string userOid)
        {
            return _persons
                .SingleOrDefaultAsync(person => userOid.Equals(person.Oid));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(Person person)
        {
            _context.Update(person);
        }
    }
}
