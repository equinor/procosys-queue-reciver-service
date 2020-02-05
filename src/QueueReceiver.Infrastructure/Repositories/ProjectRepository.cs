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
        private readonly DbSet<Project> _projects;

        public ProjectRepository(QueueReceiverServiceContext context)
            => _projects = context.Projects;

        public Task<List<Project>> GetParentProjectsByPlantAsync(string plantId)
        {
            return _projects
                .Where(project =>
                    project.ParentProjectId == null
                    && project.PlantId.Equals(plantId)
                    && !project.IsVoided)
                .ToListAsync();
        }
    }
}
