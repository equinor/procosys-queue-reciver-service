using QueueReceiverService.Models;
using System.Threading.Tasks;

namespace QueueReceiverService.Repositories
{
    public interface IPlantRepository
    {
        Task<bool> Exists(string plantOid);
        Task<string> GetPlantIdByOid(string plantOid);
    }
}
