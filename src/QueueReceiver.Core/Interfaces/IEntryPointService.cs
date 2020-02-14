using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IEntryPointService
    {
        Task InitializeQueueAsync();
        Task DisposeQueueAsync();
    }
}
