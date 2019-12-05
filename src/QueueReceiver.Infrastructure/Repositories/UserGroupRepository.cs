using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public UserGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<long> FindIdByUserGroupName(string name)
            => await _context.UserGroups
            .Where(userGroup => name.Equals(userGroup.Name))
            .Select(userGroup => userGroup.Id)
            .SingleAsync();
    }
}
