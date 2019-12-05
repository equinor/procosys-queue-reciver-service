using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantRepository
    {
        Task<string?> GetPlantIdByOid(string plantOid);
    }
}
