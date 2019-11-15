using System.Threading.Tasks;

namespace QueueReciverService.Services
{
    public interface IProjectService
    {
        ValueTask<bool> GiveAccessToPlant(string oid, string plantId);
        ValueTask<bool> RemoveAccessToPlant(string id, string plantId);
    }
}