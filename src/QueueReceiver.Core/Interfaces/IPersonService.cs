using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> UpdateWithOidIfNotFound(string userOid);
        Task<Person> CreateIfNotExist(string userOid);
        Task<Person?> FindPersonByOidAsync(string userOid);
        Task<long> GetPersonIdByOidAsync(string userOid);
        IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids);
        Task<Person?> FindAndUpdateAsync(AdPerson adPerson);
        Task<IEnumerable<string>> GetMembersWithOidAndAccessToPlant(string plantId);
    }
}