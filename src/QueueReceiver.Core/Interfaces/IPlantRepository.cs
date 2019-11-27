using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantRepository
    {
        Task<bool> Exists(string plantOid);
        Task<string> GetPlantIdByOid(string plantOid);
    }
}
