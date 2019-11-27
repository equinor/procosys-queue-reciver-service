using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IPersonProjectRepository _personProjectRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonUserGroupRepository _personUserGroupRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public ProjectService(
            IPersonProjectRepository personProjectRepository,
            IProjectRepository projectRepository,
            IPersonUserGroupRepository personUserGroupRepository,
            IUserGroupRepository userGroupRepository)
        {
            _personProjectRepository = personProjectRepository;
            _projectRepository = projectRepository;
            _personUserGroupRepository = personUserGroupRepository;
            _userGroupRepository = userGroupRepository;
        }

        public async Task GiveProjectAccessToPlant(long personId, string plantId)
        {
            List<Project> projects = await _projectRepository.GetParentProjectsByPlant(plantId);
            bool updated = false;
            projects.ForEach(async project=>
            {
                var projectId = project.ProjectId;
                var personProject = await _personProjectRepository.GetAsync(projectId, personId);
                if(personProject == null)
                {
                    await _personProjectRepository.AddAsync(projectId, personId);
                    updated = true;
                }
                else if (personProject.IsVoided)
                {
                    personProject.IsVoided = false;
                    _personProjectRepository.Update(personProject);
                    updated = true;
                }
            });

            if (updated)
            {
                long userGroupId = await _userGroupRepository.FindIdByUserGroupName("READ"); //TODO add env variable
                await _personUserGroupRepository.AddAsync(userGroupId, plantId, personId);
                //TODO add default privilege if does not exists
                await _personProjectRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveAccessToPlant(long personId, string plantId)
        {
            _personProjectRepository.VoidPersonProjects(plantId, personId);
            await _personProjectRepository.SaveChangesAsync();
        }
    }
}
