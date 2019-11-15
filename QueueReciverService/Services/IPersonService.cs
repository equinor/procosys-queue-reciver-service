using System.Threading.Tasks;
using QueueReciverService.Models;

namespace QueueReciverService.Services
{
    public interface IPersonService
    {
        Task<Person> FindByOid(string userOid);
        Task<Person> Add(Person graphPerson);
        Task Update(Person person);
        Task<bool> SaveChangesAsync();
        Task<Person> FindByEmail(string email);
        Task<Person> FindByUsername(string username);
    }
}