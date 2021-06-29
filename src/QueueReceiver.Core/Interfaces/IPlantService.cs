using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantService
    {
        Task<string?> GetPlantIdAsync(string plantOid);
        List<Plant> GetAllPlants();
        Plant GetPlant(string plantId);
    }
}