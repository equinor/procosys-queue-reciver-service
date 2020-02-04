using QueueReceiver.Core.Interfaces;
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

        public async Task<string?> GetPlantIdAsync(string plantOid)
            => await _plantRepository.GetPlantIdByOidAsync(plantOid);
    }
}