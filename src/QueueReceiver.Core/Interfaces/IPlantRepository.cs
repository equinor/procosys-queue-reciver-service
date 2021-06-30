using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantRepository
    {
        Task<string?> GetPlantIdByOidAsync(string plantOid);
        List<Plant> GetAllPlants();
        Plant GetPlant(string plantId);
    }
}
