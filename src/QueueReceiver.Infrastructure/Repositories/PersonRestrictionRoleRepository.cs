using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonRestrictionRoleRepository : IPersonRestrictionRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRestrictionRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddIfNotExistAsync(string plantId, string restrictionRole, long personId)
        {
            var prr = new PersonRestrictionRole(plantId, restrictionRole, personId);

            var exists = _context.PersonRestrictionRoles.Find(prr.RestrictionRole, prr.PersonId, prr.PlantId) != null;

            if (!exists)
            {
                await _context.PersonRestrictionRoles.AddAsync(prr);
            }
        }
    }
}