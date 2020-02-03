using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> UpdateWithOidIfNotFound(string userOid);
        Task<Person> CreateIfNotExist(string userOid);
        Task<Person?> FindByOid(string userOid);
        IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids);
        Task<Person?> FindAndUpdate(AdPerson aadPerson);
    }
}