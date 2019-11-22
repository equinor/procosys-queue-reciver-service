using QueueReceiverService.Models;
using QueueReceiverService.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiverService.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IPersonProjectRepository _personProjectRepository;
        private readonly IProjectRepository _projectRepository;

        public ProjectService(
            IPersonProjectRepository personProjectRepository,
            IProjectRepository projectRepository)
        {
            _personProjectRepository = personProjectRepository;
            _projectRepository = projectRepository;
        }

        public async Task GiveProjectAccessToPlant(long personId, string plantId)
        {
            List<Project> projects = await _projectRepository.GetProjectsByPlant(plantId);

            projects.ForEach(async project
                => await _personProjectRepository.AddIfNotExists(personId, project.ProjectId));

            await _personProjectRepository.SaveChangesAsync();
        }

        public async Task RemoveAccessToPlant(long personId, string plantId)
        {
            _personProjectRepository.RemovePersonProjects(plantId, personId);
            await _personProjectRepository.SaveChangesAsync();
        }
    }
}
