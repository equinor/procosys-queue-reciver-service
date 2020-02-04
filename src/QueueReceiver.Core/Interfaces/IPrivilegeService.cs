using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPrivilegeService
    {
        Task GivePrivilegesAsync(string plantId, long personId);
    }
}
