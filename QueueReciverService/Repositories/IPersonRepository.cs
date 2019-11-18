using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Repositories
{
    public interface IPersonRepository
    {
        Task<Person> FindByUserOid(string userOid);
        Task<Person> FindByUserEmail(string userEmail);
        Task<Person> FindByUsername(string userName);
        Task<Person> AddPerson(Person person);
        void Update(Person person);
        Task<bool> SaveChangesAsync();
    }
}