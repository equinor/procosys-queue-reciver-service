using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class PersonProjectService : IPersonProjectService
    {
        private readonly IPersonProjectRepository _personProjectRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IPrivilegeService _privilegeService;
        private readonly IPersonProjectHistoryRepository _personProjectHistoryRepository;
        

        public PersonProjectService(
            IPersonProjectRepository personProjectRepository,
            IProjectRepository projectRepository,
            IPrivilegeService privilegeService,
            IPersonProjectHistoryRepository personProjectHistoryRepository
        )
        {
            _personProjectRepository = personProjectRepository;
            _projectRepository = projectRepository;
            _privilegeService = privilegeService;
            _personProjectHistoryRepository = personProjectHistoryRepository;
        }

        public async Task GiveProjectAccessToPlant(long personId, string plantId)
        {
            var updated = false;
            var unvoided = false;

            var personProjectHistory = PersonProjectHistoryHelper.CreatePersonProjectHistory(personId);
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
                    unvoided = true;
                }
            });

            if (updated)
            {
               await _privilegeService.GivePrivlieges(plantId, personId);

                projects.ForEach(p =>
                {
                    PersonProjectHistoryHelper.LogAddAccess(personId, personProjectHistory, p.ProjectId);

                    if (p.IsMainProject)
                    {
                        PersonProjectHistoryHelper.LogDefaultUserGroup(personId, personProjectHistory, p.ProjectId);
                    }
                });
            }

            if (unvoided)
            {
                projects.ForEach(p => PersonProjectHistoryHelper.LogUnvoidProjects(personId, personProjectHistory, p.ProjectId));
            }

            if (unvoided || updated)
            {
                await _personProjectHistoryRepository.AddAsync(personProjectHistory);
            }
        }

        public void RemoveAccessToPlant(long personId, string plantId)
        {
            var personProjectHistory = PersonProjectHistoryHelper.CreatePersonProjectHistory(personId);
            var projects = _personProjectRepository.VoidPersonProjects(plantId, personId).Select(pp => pp.Project!).ToList();
            projects.ForEach(p => PersonProjectHistoryHelper.LogVoidProjects(personId, personProjectHistory, p.ProjectId));
        }
    }
}