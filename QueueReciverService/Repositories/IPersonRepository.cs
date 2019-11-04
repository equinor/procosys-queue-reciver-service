using QueueReciverService.Models;
using System.Threading.Tasks;

namespace QueueReciverService.Repositories
{
    public interface IPersonRepository
    {
        Task<Person> FindByUserOid(string userOid);
        Task<Person> FindByUserEmail(string userEmail);
        Task<Person> FindByUserName(string userName);
        Task<Person> AddPerson(Person person);
        void Update(Person person);
        Task<bool> SaveChangesAsync();
    }
}