using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public interface IPlantService
    {
        Task<bool> Exists(string accessInfoPlantOid);
    }
}