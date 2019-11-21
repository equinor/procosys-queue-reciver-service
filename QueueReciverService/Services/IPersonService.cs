using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public interface IPersonService
    {
        Task<Person?> FindByOid(string userOid);
        Task<Person> FindOrCreate(string userOid);
    }
}