using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantService
    {
        Task<bool> Exists(string accessInfoPlantOid);
        Task<string> GetPlantId(string plantOid);
    }
}