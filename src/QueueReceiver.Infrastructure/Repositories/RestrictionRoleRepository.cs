﻿using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class RestrictionRoleRepository : IRestrictionRoleRepository
    {
        private readonly DbSet<RestrictionRole> _restrictionRoles;

        public RestrictionRoleRepository(QueueReceiverServiceContext context)
            => _restrictionRoles = context.RestrictionRoles;

        public async Task<string> FindRestrictionRoleAsync(string name, string plant) =>
            await _restrictionRoles.Where(restrictionRole
                    => name.Equals(restrictionRole.RestrictionRoleId) && plant.Equals(restrictionRole.PlantId))
                    .Select(restrictionRole => restrictionRole.RestrictionRoleId)
                    .SingleAsync();
    }
}