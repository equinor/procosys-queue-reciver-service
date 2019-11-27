using System.Threading.Tasks;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectRepository
    {
        Task<int> SaveChangesAsync();
        void VoidPersonProjects(string plantId, long personId);
        void Update(PersonProject personProject);
        Task AddAsync(long projectId, long personId);
        Task<PersonProject> GetAsync(long personId, long projectId);
    }
}