using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonUserGroupRepository : IPersonUserGroupRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbContextSettings _settings;

        public PersonUserGroupRepository(ApplicationDbContext context, DbContextSettings settings)
        {
            _context = context;
            _settings = settings;
        }

       public async Task AddIfNotExistAsync(long userGroupId, string plantId, long personId)
        {
            var createdById = _settings.PersonProjectCreatedId;
            var pug = new PersonUserGroup(personId, userGroupId, plantId, createdById);

            var exists = _context.PersonUserGroups.Find(pug.PlantId, pug.PersonId, pug.UserGroupId) != null;

            if (!exists)
            {
                await _context.PersonUserGroups.AddAsync(pug);
            }
        }
    }
}
