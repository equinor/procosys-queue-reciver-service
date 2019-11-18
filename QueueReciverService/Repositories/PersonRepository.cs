using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueReceiverService.Data;
using QueueReceiverService.Models;

namespace QueueReceiverService.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person> AddPerson(Person person)
        {
           await _context.Persons.AddAsync(person);
           return person;
        }

        public Task<Person> FindByUserEmail(string userEmail)
        {
           return _context.Persons
                .SingleOrDefaultAsync(person => person.Email.Equals(userEmail));
        }

        public Task<Person> FindByUsername(string userName)
        {
            return _context.Persons
                .SingleOrDefaultAsync(person => person.UserName.Equals(userName));
        }

        public Task<Person> FindByUserOid(string userOid)
        {
            return _context.Persons
                .SingleOrDefaultAsync(person => person.Oid.Equals(userOid));
        }

        public async Task<bool> SaveChangesAsync()
        {
          return await _context.SaveChangesAsync() > 0;
        }

        public void Update(Person person)
        {
             _context.Update(person);
        }
    }
}
