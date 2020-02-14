using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectService
    {
        Task GiveProjectAccessToPlantAsync(long personId, string plantId);
        void RemoveAccessToPlant(long personId, string plantId);
    }
}