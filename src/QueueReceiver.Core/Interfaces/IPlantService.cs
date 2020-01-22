using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantService //TODO: Use type Guid in methods
    {
        Task<string?> GetPlantId(string plantOid);
    }
}