using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonUserGroupRepository : IPersonUserGroupRepository
    {
        private readonly ApplicationDbContext context;
        private readonly DbContextSettings _settings;

        public PersonUserGroupRepository(ApplicationDbContext context, DbContextSettings settings)
        {
            this.context = context;
            _settings = settings;
        }

       public async Task AddAsync(long userGroupId, string plantId, long personId)
        {
            var createdById = _settings.PersonProjectCreatedId;
            var personUserGroup = new PersonUserGroup(personId, userGroupId, plantId, createdById);
            await context.PersonUserGroups.AddAsync(personUserGroup);
        }
    }
}
