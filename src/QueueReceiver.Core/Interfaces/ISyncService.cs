using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface ISyncService
    {
        Task StartAccessSync(List<string> plantList, bool removeUserAccess);
    }
}
