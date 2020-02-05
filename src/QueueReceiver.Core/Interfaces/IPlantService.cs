using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantService
    {
        Task<string?> GetPlantId(string plantOid);
        IEnumerable<string> GetAllGroupOids();
        List<Plant> GetAllPlants();
        List<string> GetAllMemberOidsByPlant(string plantId);
    }
}