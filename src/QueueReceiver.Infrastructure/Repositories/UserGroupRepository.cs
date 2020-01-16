using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly DbSet<UserGroup> _userGroups;

        public UserGroupRepository(ApplicationDbContext context)
            => _userGroups = context.UserGroups;

        public async Task<long> FindIdByUserGroupName(string name)
            => await _userGroups
            .Where(userGroup => name.Equals(userGroup.Name))
            .Select(userGroup => userGroup.Id)
            .SingleAsync();
    }
}
