using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            _persons = _context.Persons;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _persons.AddAsync(person);
            return person;
        }

        public async Task<Person?> FindByNameAndMobileNumber(string mobileNumber, string givenName, string surname)
        {
           mobileNumber = mobileNumber.Replace(" ", "");

            return await _persons.AsNoTracking().FirstOrDefaultAsync(p =>
            p.MobilePhoneNumber != null
             &&  (mobileNumber.Equals(p.MobilePhoneNumber.Replace(" ", ""))
                || mobileNumber.Equals("+47" + p.MobilePhoneNumber.Replace(" ", "")))
             && givenName.Equals(p.FirstName)
             && surname.Equals(p.LastName));

        }

        public async Task<Person> FindByUserEmail(string userEmail) =>
           await _persons.SingleOrDefaultAsync(person =>
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

        public IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids)
        {
            var withOid = _persons.Where(p => p.Oid != null).Select(p => p.Oid!).AsNoTracking().ToAsyncEnumerable();
            return oids.ToAsyncEnumerable().Except(withOid).ToEnumerable();

            //var result = new List<string>();
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //Console.WriteLine($"code sort takes {sw.ElapsedMilliseconds}ms");

            //sw.Restart();
            //for (int i = 0; i < oids.Count; i += 1000)
            //{
            //    int count = ((oids.Count - i) < 1000) ? (oids.Count - i) - 1 : 1000;
            //     var toAdd = await _persons
            //    .Where(p => p.Oid != null && !oids.GetRange(i,count).Contains(p.Oid))
            //    .Select(p => p.Oid)
            //    .ToListAsync();
            //    result.AddRange(toAdd);
            //}
            //Console.WriteLine($"Partitioning takes {sw.ElapsedMilliseconds}ms");
        }

        public async Task<Person?> FindByUserOid(string userOid) =>
           await _persons.FirstOrDefaultAsync(person =>
                userOid.Equals(person.Oid, OrdinalIgnoreCase));

        //public void BulkUpdate(IList<Person> persons)
        //{
        //    _context.BulkUpdate(persons);
        //}

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Update(Person person)
            => _context.Persons.Update(person);

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
