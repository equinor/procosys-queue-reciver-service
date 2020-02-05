﻿using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PlantRepository : IPlantRepository
    {
        private readonly DbSet<Plant> _plants;

        public PlantRepository(QueueReceiverServiceContext context)
            => _plants = context.Plants;

        public IEnumerable<string> GetAllInternalAndAffiliateOids()
        {
            var affiliates = _plants.Where(plant => plant.AffiliateGroupId != null).Select(plant => plant.AffiliateGroupId).AsNoTracking();
            var inter = _plants.Where(plant => plant.InternalGroupId != null).Select(plant => plant.InternalGroupId).AsNoTracking();
            return affiliates.Concat(inter);
        }

        public Task<string?> GetPlantIdByOid(string plantOid)
            => _plants
                .Where(plant => plantOid.Equals(plant.InternalGroupId) || plantOid.Equals(plant.AffiliateGroupId))
                .Select(plant => plant.PlantId)
                .SingleOrDefaultAsync<string?>();

        public List<Plant> GetAllPlants() => _plants.ToList(); //where clause aff and int group ids

        public List<string> GetMemberOidsByPlant(string plantId)
        {
            var affiliates = _plants.Where(plant => plant.AffiliateGroupId != null && plant.PlantId == plantId)
                .Select(plant => plant.AffiliateGroupId)
                .AsNoTracking();

            var inter = _plants.Where(plant => plant.InternalGroupId != null && plant.PlantId == plantId)
                .Select(plant => plant.InternalGroupId)
                .AsNoTracking();
            
            return affiliates.Concat(inter).ToList();
        }
    }
}