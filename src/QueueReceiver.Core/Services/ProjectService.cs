using QueueReceiver.Core.Interfaces;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class ProjectService : IProjectService
    {
        private const string DefaultUserGroup = "READ";
        private readonly IPersonProjectRepository _personProjectRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonUserGroupRepository _personUserGroupRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IPersonRestrictionRoleRepository _personRestrictionRoleRepository;
        private readonly IRestrictionRoleRepository _restrictionRoleRepository;

        public ProjectService(
            IPersonProjectRepository personProjectRepository,
            IProjectRepository projectRepository,
            IPersonUserGroupRepository personUserGroupRepository,
            IUserGroupRepository userGroupRepository,
            IPersonRestrictionRoleRepository personRestrictionRoleRepository,
            IRestrictionRoleRepository restrictionRoleRepository
        )
        {
            _personProjectRepository = personProjectRepository;
            _projectRepository = projectRepository;
            _personUserGroupRepository = personUserGroupRepository;
            _userGroupRepository = userGroupRepository;
            _personRestrictionRoleRepository = personRestrictionRoleRepository;
            _restrictionRoleRepository = restrictionRoleRepository;
        }

        public async Task GiveProjectAccessToPlant(long personId, string plantId)
        {
            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);
            var updated = false;
            projects.ForEach(async project =>
            {
                var projectId = project.ProjectId;
                var personProject = await _personProjectRepository.GetAsync(projectId, personId);
                // if this person does not already have this access
                if (personProject == null)
                {
                    // add them
                    await _personProjectRepository.AddAsync(projectId, personId);
                    //TODO PersonProjectHistory
                    updated = true;
                }
                // if there was one already, check if they're voided
                // if they are,
                else if (personProject.IsVoided)
                {
                    // unvoid them and send update command to db
                    personProject.IsVoided = false;
                    _personProjectRepository.Update(personProject);
                    //TODO PersonProjectHistory
                    updated = true;
                }
            });

            // if new access is added or previous has been unvoided,
            if (updated)
            {
                var userGroupId = await _userGroupRepository.FindIdByUserGroupName(DefaultUserGroup);
                await _personUserGroupRepository.AddIfNotExistAsync(userGroupId, plantId, personId);

                var restrictionRole = await _restrictionRoleRepository.FindRestrictionRole("NO_RESTRICTIONS", plantId);
                await _personRestrictionRoleRepository.AddIfNotExistAsync(plantId, restrictionRole, personId);

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