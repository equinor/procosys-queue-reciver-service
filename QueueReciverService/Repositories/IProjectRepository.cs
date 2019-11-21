using System.Collections.Generic;
using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Repositories
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetProjectsByPlant(string plantId);
    }
}
