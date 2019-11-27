using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext context;

        public ProjectRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task<List<Project>> GetParentProjectsByPlant(string plantId)
        {
            return context.Projects
                .Where(project =>
                    project.ParentProjectId == null
                    && project.PlantId.Equals(plantId)
                    && !project.IsVoided)
                .ToListAsync();
        }
    }
}
