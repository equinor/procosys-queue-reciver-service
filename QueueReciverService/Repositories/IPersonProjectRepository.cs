using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueReceiverService.Data;
using QueueReceiverService.Models;

namespace QueueReceiverService.Repositories
{
    public interface IPersonProjectRepository
    {
        Task AddIfNotExists(long personId, long id);
        Task<int> SaveChangesAsync();
        void RemoveIfExists(long personId, long projectId);
        void RemovePersonProjects(string plantId, long personId);
    }
}