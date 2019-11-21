using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueReceiverService.Data;
using QueueReceiverService.Models;

namespace QueueReceiverService.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext context;

        public ProjectRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task<List<Project>> GetProjectsByPlant(string plantId)
        {
            return context.Projects
                .Where(project => project.PlantId.Equals(plantId)
                    && project.IsVoided)
                .ToListAsync();
        }
    }
}
