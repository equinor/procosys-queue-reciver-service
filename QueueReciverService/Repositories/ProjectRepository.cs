using System.Collections.Generic;
using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        public Task<List<Project>> GetProjectsByPlant(string plantId)
        {
            throw new System.NotImplementedException();
        }
    }
}
