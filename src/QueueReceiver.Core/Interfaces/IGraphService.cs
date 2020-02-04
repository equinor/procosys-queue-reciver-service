using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IGraphService
    {
        Task<AdPerson?> GetAdPersonByOid(string userOid);
        Task<IEnumerable<string>> GetMemberOids(string groupOid);
    }
}