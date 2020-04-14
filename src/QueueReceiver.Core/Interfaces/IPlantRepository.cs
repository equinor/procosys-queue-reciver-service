using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantRepository
    {
        Task<string?> GetPlantIdByOidAsync(string plantOid);
        IEnumerable<string> GetAllInternalAndAffiliateOids();
        List<Plant> GetAllPlants();
        List<string> GetMemberOidsByPlant(string plantId);
        Plant GetPlant(string plantId);
    }
}
