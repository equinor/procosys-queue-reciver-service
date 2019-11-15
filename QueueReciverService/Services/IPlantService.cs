using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public interface IPlantService
    {
        ValueTask<bool> Exists(string accessInfoPlantOid);
    }
}