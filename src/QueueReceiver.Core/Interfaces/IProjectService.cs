using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IProjectService
    {
        Task GiveProjectAccessToPlant(long personId, string plantId);
        void RemoveAccessToPlant(long personId, string plantId);
    }
}