using QueueReceiver.Core.Models;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IAccessService
    {
        Task HandleRequest(AccessInfo accessInfo);
    }
}
