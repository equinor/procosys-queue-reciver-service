using QueueReceiver.Core.Models;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> FindByOid(string userOid);
        Task<Person> FindOrCreate(string userOid);
    }
}