using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public interface IAccessService
    {
        ValueTask<bool> HandleRequest(AccessInfo accessInfo);
    }
}
