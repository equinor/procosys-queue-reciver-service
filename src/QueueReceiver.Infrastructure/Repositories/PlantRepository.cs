using Microsoft.EntityFrameworkCore;
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

        public Task<string?> GetPlantIdByOidAsync(string plantOid)
            => _plants
                .Where(plant => plantOid.Equals(plant.InternalGroupId) || plantOid.Equals(plant.AffiliateGroupId))
                .Where(plant => !plant.IsVoided)
                .Select(plant => plant.PlantId)
                .SingleOrDefaultAsync<string?>();

        public List<Plant> GetAllPlants() => _plants.ToList(); //where clause aff and int group ids

        public Plant GetPlant(string plantId)
            => _plants.SingleOrDefault(plant => plant.PlantId == plantId);
    }
}