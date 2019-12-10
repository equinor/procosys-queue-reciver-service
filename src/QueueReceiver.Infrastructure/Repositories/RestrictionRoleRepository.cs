using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class RestrictionRoleRepository : IRestrictionRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RestrictionRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> FindRestrictionRole(string name, string plant) =>
            await _context.RestrictionRoles.Where(restrictionRole 
                    => name.Equals(restrictionRole.RestrictionRoleId) && plant.Equals(restrictionRole.PlantId))
                    .Select(restrictionRole => restrictionRole.RestrictionRoleId)
                    .SingleAsync();
    }
}