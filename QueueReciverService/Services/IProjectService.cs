using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public interface IProjectService
    {
        ValueTask<bool> GiveAccessToPlant(string oid, string plantId);
        ValueTask<bool> RemoveAccessToPlant(string id, string plantId);
    }
}