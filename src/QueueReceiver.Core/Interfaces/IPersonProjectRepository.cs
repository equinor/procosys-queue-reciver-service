using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectRepository
    {
        Task AddIfNotExists(long personId, long id);
        Task<int> SaveChangesAsync();
        void RemoveIfExists(long personId, long projectId);
        void RemovePersonProjects(string plantId, long personId);
    }
}