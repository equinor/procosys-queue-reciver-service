using QueueReceiver.Core.Models;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IGraphService
    {
        Task<AdPerson> GetPersonByOid(string userOid);
    }
}