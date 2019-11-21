using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueReceiverService.Data;

namespace QueueReceiverService.Repositories
{
    public interface IPersonProjectRepository
    {
        Task<int> AddIfNotExists(long personId, long id);
        Task<int> SaveChangesAsync();
        int RemoveIfExists(long personId, long projectId);
    }
}