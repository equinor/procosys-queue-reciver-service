using QueueReciverService.Models;
using System.Threading.Tasks;

namespace QueueReciverService.Services
{
    public interface IGraphService
    {
        Task<Person> GetPersonByOid(string userOid);
    }
}