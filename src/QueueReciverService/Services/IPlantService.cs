using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public interface IPlantService
    {
        Task<bool> Exists(string accessInfoPlantOid);
        Task<string> GetPlantId(string plantOid);
    }
}