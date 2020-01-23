using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantService
    {
        Task<string?> GetPlantId(string plantOid);
    }
}