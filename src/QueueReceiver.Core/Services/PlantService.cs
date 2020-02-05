using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class PlantService : IPlantService
    {
        private readonly IPlantRepository _plantRepository;

        public PlantService(IPlantRepository plantRepository)
            => _plantRepository = plantRepository;

        public IEnumerable<string> GetAllGroupOids()
            => _plantRepository.GetAllInternalAndAffiliateOids();

        public async Task<string?> GetPlantId(string plantOid)
            => await _plantRepository.GetPlantIdByOid(plantOid);

        public List<Plant> GetAllPlants()
            => _plantRepository.GetAllPlants();

        public List<string> GetAllMemberOidsByPlant(string plantId)
            => _plantRepository.GetMemberOidsByPlant(plantId);

    }
}