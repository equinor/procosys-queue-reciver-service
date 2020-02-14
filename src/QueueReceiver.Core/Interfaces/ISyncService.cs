using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface ISyncService
    {
        Task ExcecuteOidUpdateAsync();
        Task ExcecuteOidSync();

        Task StartAccessSync();
    }
}
