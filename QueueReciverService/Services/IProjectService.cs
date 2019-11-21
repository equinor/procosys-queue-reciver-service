using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public interface IProjectService
    {
        Task GiveProjectAccessToPlant(long personId, string plantId);
        Task RemoveAccessToPlant(long personId, string plantId);
    }
}