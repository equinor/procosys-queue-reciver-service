using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> FindOrUpdate(string userOid);
        Task<Person> CreateIfNotExist(string userOid);
        Task<Person?> FindByOid(string userOid);
        IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids);
        Task FindAndUpdate(AdPerson aadPerson);
        //Task FindAndUpdate(AdPerson aadPerson);
    }
}