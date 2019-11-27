﻿using QueueReceiver.Core.Interfaces;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class PlantService : IPlantService
    {
        private readonly IPlantRepository _plantRepository;

        public PlantService(IPlantRepository plantRepository)
        {
            _plantRepository = plantRepository;
        }

        public async Task<bool> Exists(string plantOid)
        {
            return await _plantRepository.Exists(plantOid);
        }

        public async Task<string> GetPlantId(string plantOid)
        {
            return await _plantRepository.GetPlantIdByOid(plantOid);
        }
    }
}