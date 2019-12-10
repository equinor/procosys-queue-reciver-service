using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
   public interface ISyncService
    {
        Task ExcecuteOidSync();
    }
}
