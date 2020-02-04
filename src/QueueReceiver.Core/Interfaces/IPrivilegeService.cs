using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPrivilegeService
    {
        Task GivePrivlieges(string plantId, long personId);
    }
}
