using QueueReceiver.Core.Models;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonService //TODO: Use Guid as parameter type in methods
    {
        Task<Person?> UpdateWithOidIfNotFound(string userOid);
        Task<Person> CreateIfNotExist(string userOid);
        Task<Person?> FindByOid(string userOid);
    }
}