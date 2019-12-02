﻿using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PlantRepository : IPlantRepository
    {
        private readonly DbSet<Plant> _plants;

        public PlantRepository(ApplicationDbContext context)
        {
            _plants = context.Plants;
        }

        public Task<bool> Exists(string oid)
        {
            return _plants.AnyAsync(plant
               => oid.Equals(plant.InternalGroupId) || oid.Equals(plant.AffiliateGroupId));
        }

        public Task<string> GetPlantIdByOid(string plantOid)
        {
            return _plants
                .Where(plant => plantOid.Equals(plant.InternalGroupId) || plantOid.Equals(plant.AffiliateGroupId))
                .Select(plant => plant.PlantId)
                .SingleOrDefaultAsync();
        }
    }
}