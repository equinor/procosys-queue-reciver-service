using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public class PlantService : IPlantService
    {
        public Task<bool> Exists(string accessInfoPlantOid)
        {
            return Task.FromResult(true); //TODO
        }
    }
}