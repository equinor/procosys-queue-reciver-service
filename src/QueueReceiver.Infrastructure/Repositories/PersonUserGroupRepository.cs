using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonUserGroupRepository : IPersonUserGroupRepository
    {
        private readonly DbContextSettings _settings;
        private readonly DbSet<PersonUserGroup> _personUserGroups;

        public PersonUserGroupRepository(QueueReceiverServiceContext context, DbContextSettings settings)
        {
            _settings = settings;
            _personUserGroups = context.PersonUserGroups;
        }

        public async Task AddIfNotExistAsync(long userGroupId, string plantId, long personId)
        {
            var createdById = _settings.PersonProjectCreatedId;
            var pug = new PersonUserGroup(personId, userGroupId, plantId, createdById);
            var exists = _personUserGroups.Find(pug.PlantId, pug.PersonId, pug.UserGroupId) != null;

            if (!exists)
            {
                await _personUserGroups.AddAsync(pug);
            }
        }
    }
}
