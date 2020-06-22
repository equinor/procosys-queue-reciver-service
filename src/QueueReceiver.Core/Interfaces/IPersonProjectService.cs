using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectService
    {
        Task<bool> GiveProjectAccessToPlantAsync(long personId, string plantId);
        Task<bool> RemoveAccessToPlant(long personId, string plantId);
    }
}