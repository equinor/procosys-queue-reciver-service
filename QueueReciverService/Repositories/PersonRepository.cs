using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueReceiverService.Data;
using QueueReceiverService.Models;

namespace QueueReceiverService.Repositories
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
                .SingleOrDefaultAsync(person => userEmail.Equals(person.Email));
        }

        public Task<Person> FindByUsername(string userName)
        {
            return _persons
                .SingleOrDefaultAsync(person => userName.Equals(person.UserName));
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
