using System.Collections.Generic;
using System.Threading.Tasks;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectRepository
    {
        List<PersonProject> VoidPersonProjects(string plantId, long personId);
        Task AddAsync(long projectId, long personId, long createdById);
        Task<PersonProject> GetAsync(long personId, long projectId);
        IEnumerable<PersonProject> GetByProject(long projectId);
        Task<bool> PersonHasNoAccess(long personId);
    }
}