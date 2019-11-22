using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public interface IAccessService
    {
        Task HandleRequest(AccessInfo accessInfo);
    }
}
