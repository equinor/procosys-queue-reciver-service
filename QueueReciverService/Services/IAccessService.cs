using QueueReciverService.Models;
using System.Threading.Tasks;

namespace QueueReciverService.Services
{
    public interface IAccessService
    {
        Task<bool> HandleRequest(AccessInfo accessInfo);
    }
}
