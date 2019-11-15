using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public interface IGraphService
    {
        Task<Person> GetPersonByOid(string userOid);
    }
}