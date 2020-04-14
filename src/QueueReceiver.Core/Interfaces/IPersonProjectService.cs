using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectService
    {
        Task GiveProjectAccessToPlantAsync(long personId, string plantId);
        Task RemoveAccessToPlant(long personId, string plantId);
    }
}