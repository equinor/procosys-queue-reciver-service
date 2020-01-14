using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> FindByOid(string userOid);
        Task<Person> FindOrCreate(string userOid);
        //Task FindAndUpdate(string memberOid);
        Task<int> SaveAsync();
        int SaveChanges();
        IEnumerable<string> GetAllNotInDb(IEnumerable<string> oids);
        Task FindAndUpdate(AdPerson aadPerson);
    }
}