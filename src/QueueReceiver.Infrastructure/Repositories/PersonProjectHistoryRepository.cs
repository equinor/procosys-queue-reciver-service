using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.EntityConfiguration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonProjectHistoryRepository : IPersonProjectHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<PersonProjectHistory> _personProjectHistories;

        public PersonProjectHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
            _personProjectHistories = context.PersonProjectHistories;
        }

        public Task<List<PersonProjectHistory>> GetPersonProjectHistoryById(int id)
        {
            return _personProjectHistories
                .Where(history =>
                    history.Id == id)
                .ToListAsync();
        }

        public async Task AddAsync(PersonProjectHistory personProjHistory)
            => await _personProjectHistories.AddAsync(personProjHistory);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}