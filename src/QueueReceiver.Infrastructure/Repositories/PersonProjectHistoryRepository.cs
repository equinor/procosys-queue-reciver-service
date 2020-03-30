using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonProjectHistoryRepository : IPersonProjectHistoryRepository
    {
        private readonly DbContextSettings _dbContextSettings;
        private readonly DbSet<PersonProjectHistory> _personProjectHistories;

        public PersonProjectHistoryRepository(QueueReceiverServiceContext context, DbContextSettings dbContextSettings)
        {
            _dbContextSettings = dbContextSettings;
            _personProjectHistories = context.PersonProjectHistories;
        }

        public Task<List<PersonProjectHistory>> GetPersonProjectHistoryByIdAsync(int id)
        {
            return _personProjectHistories
                .Where(history =>
                    history.Id == id)
                .ToListAsync();
        }

        public async Task AddAsync(PersonProjectHistory personProjectHistory)
        {
            personProjectHistory.UpdatedBy = _dbContextSettings.PersonProjectCreatedId;
            personProjectHistory.UpdatedByUserName = _dbContextSettings.PersonProjectCreatedUsername;

            foreach (var operation in personProjectHistory.PersonProjectHistoryOperations)
            {
                operation.UpdatedByUser = _dbContextSettings.PersonProjectCreatedUsername;
            }

            await _personProjectHistories.AddAsync(personProjectHistory);
        }
    }
}