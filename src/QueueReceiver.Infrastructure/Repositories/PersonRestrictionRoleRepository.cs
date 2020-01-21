using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.EntityConfiguration;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonRestrictionRoleRepository : IPersonRestrictionRoleRepository
    {
        private readonly DbSet<PersonRestrictionRole> _personRestrictionRoles;

        public PersonRestrictionRoleRepository(QueueReceiverServiceContext context) 
            => _personRestrictionRoles = context.PersonRestrictionRoles;

        public async Task AddIfNotExistAsync(string plantId, string restrictionRole, long personId)
        {
            var prr = new PersonRestrictionRole(plantId, restrictionRole, personId);

            var exists = _personRestrictionRoles.Find(prr.PlantId, prr.RestrictionRole, prr.PersonId) != null;

            if (!exists)
            {
                await _personRestrictionRoles.AddAsync(prr);
            }
        }
    }
}