using Microsoft.EntityFrameworkCore;
using QueueReciverService.Data;
using QueueReciverService.Models;
using System.Threading.Tasks;

namespace QueueReciverService.Repositories
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
           return _context.Persons.SingleOrDefaultAsync(person => person.Email.Equals(userEmail));
        }

        public Task<Person> FindByUsername(string userName)
        {
            throw new System.NotImplementedException();
        }

        public Task<Person> FindByUserOid(string userOid)
        {
            return _context.Persons.SingleOrDefaultAsync(person => person.Oid.Equals(userOid));
        }

        public async Task<bool> SaveChangesAsync()
        {
          return await _context.SaveChangesAsync() > 0;
        }

        public async Task Update(Person person)
        {
             _context.Update(person);
        }
    }
}
