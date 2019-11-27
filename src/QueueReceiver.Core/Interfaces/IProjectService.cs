using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IProjectService
    {
        Task GiveProjectAccessToPlant(long personId, string plantId);
        Task RemoveAccessToPlant(long personId, string plantId);
    }
}