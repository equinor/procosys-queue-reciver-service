using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IEntryPointService
    {
        Task InitializeQueue();
        Task DisposeQueue();
    }
}
