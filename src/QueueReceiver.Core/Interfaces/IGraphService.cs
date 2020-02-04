using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IGraphService
    {
        Task<AdPerson?> GetAdPersonByOidAsync(string userOid);
        Task<IEnumerable<string>> GetMemberOidsAsync(string groupOid);
    }
}