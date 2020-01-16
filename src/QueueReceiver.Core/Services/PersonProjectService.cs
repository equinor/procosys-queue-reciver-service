using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Interfaces;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class PersonProjectService : IPersonProjectService
    {
        private readonly IPersonProjectRepository _personProjectRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonUserGroupRepository _personUserGroupRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IPersonRestrictionRoleRepository _personRestrictionRoleRepository;
        private readonly IRestrictionRoleRepository _restrictionRoleRepository;
        private readonly IPersonProjectHistoryRepository _personProjectHistoryRepository;
        private readonly IPersonProjectHistoryService _personProjectHistoryService;

        public PersonProjectService(
            IPersonProjectRepository personProjectRepository,
            IProjectRepository projectRepository,
            IPersonUserGroupRepository personUserGroupRepository,
            IUserGroupRepository userGroupRepository,
            IPersonRestrictionRoleRepository personRestrictionRoleRepository,
            IRestrictionRoleRepository restrictionRoleRepository,
            IPersonProjectHistoryRepository personProjectHistoryRepository,
            IPersonProjectHistoryService personProjectHistoryService
        )
        {
            _personProjectRepository = personProjectRepository;
            _projectRepository = projectRepository;
            _personUserGroupRepository = personUserGroupRepository;
            _userGroupRepository = userGroupRepository;
            _personRestrictionRoleRepository = personRestrictionRoleRepository;
            _restrictionRoleRepository = restrictionRoleRepository;
            _personProjectHistoryRepository = personProjectHistoryRepository;
            _personProjectHistoryService = personProjectHistoryService;
        }

        public async Task GiveProjectAccessToPlant(long personId, string plantId)
        { 
            var updated = false;
            var unvoided = false;

            var personProjectHistory = _personProjectHistoryService.CreatePersonProjectHistory(personId);
            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);
           
            projects.ForEach(async project =>
            {
                var projectId = project.ProjectId;
                var personProject = await _personProjectRepository.GetAsync(projectId, personId);

                if (personProject == null)
                {
                    await _personProjectRepository.AddAsync(projectId, personId);
                    updated = true;
                }

                else if (personProject.IsVoided)
                {
                    personProject.IsVoided = false;
                    _personProjectRepository.Update(personProject);
                    unvoided = true;
                }
            });

            if (updated)
            {
                var userGroupId = await _userGroupRepository.FindIdByUserGroupName(PersonProjectConstants.DefaultUserGroup);
                await _personUserGroupRepository.AddIfNotExistAsync(userGroupId, plantId, personId);

                var restrictionRole = await _restrictionRoleRepository.FindRestrictionRole(PersonProjectConstants.DefaultRestrictionRole, plantId);
                await _personRestrictionRoleRepository.AddIfNotExistAsync(plantId, restrictionRole, personId);

                projects.ForEach(p =>
                {
                    _personProjectHistoryService.LogAddAccess(personId, personProjectHistory, p.ProjectId);
                    
                    if (p.IsMainProject)
                    {
                        _personProjectHistoryService.LogDefaultUserGroup(personId, personProjectHistory, p.ProjectId);
                    }
                });
            }

            if (unvoided)
            { 
                projects.ForEach(p => _personProjectHistoryService.LogUnvoidProjects(personId, personProjectHistory, p.ProjectId));
            }

            if (unvoided || updated)
            {
                await _personProjectHistoryRepository.AddAsync(personProjectHistory);
                await _personProjectRepository.SaveChangesAsync();
            }
        }

        public void RemoveAccessToPlant(long personId, string plantId)
        {
            var personProjectHistory = _personProjectHistoryService.CreatePersonProjectHistory(personId); 
            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);
            _personProjectRepository.VoidPersonProjects(plantId, personId);

            projects.ForEach(p => _personProjectHistoryService.LogVoidProjects(personId, personProjectHistory, p.ProjectId));
            
            await _personProjectRepository.SaveChangesAsync();
        }
    }
}