using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public interface IProjectService
    {
        Task<bool> GiveAccessToPlant(string oid, string plantId);
        Task<bool> RemoveAccessToPlant(string id, string plantId);
    }
}