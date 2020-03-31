using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Collections.Generic;
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
        private readonly IPersonService _personService;
        private readonly ILogger<PersonProjectService> _logger;

        public PersonProjectService(
            IPersonProjectRepository personProjectRepository,
            IProjectRepository projectRepository,
            IPrivilegeService privilegeService,
            IPersonProjectHistoryRepository personProjectHistoryRepository,
            IPersonService personService,
            ILogger<PersonProjectService> logger)
        {
            _personProjectRepository = personProjectRepository;
            _projectRepository = projectRepository;
            _privilegeService = privilegeService;
            _personProjectHistoryRepository = personProjectHistoryRepository;
            _personService = personService;
            _logger = logger;
        }

        public async Task GiveProjectAccessToPlantAsync(long personId, string plantId)
        {
            var personProjectHistory = PersonProjectHistoryHelper.CreatePersonProjectHistory(personId);
            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);

            var (updated, unvoided) = await UpdatePersonProjectsAsync(personId, projects);

            if (updated)
            {
                await UpdatePrivlegesAsync(personId, plantId, personProjectHistory, projects);
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

        public async Task RemoveAccessToPlant(long personId, string plantId)
        {
            var personProjectHistory = PersonProjectHistoryHelper.CreatePersonProjectHistory(personId);
            var projects = _personProjectRepository.VoidPersonProjects(plantId, personId).Select(pp => pp.Project!).ToList();
            projects.ForEach(p => PersonProjectHistoryHelper.LogVoidProjects(personId, personProjectHistory, p.ProjectId));

            if (projects.Count > 0)
            {
                await _personProjectHistoryRepository.AddAsync(personProjectHistory);
            }
        }

        private async Task<(bool updated, bool unvoided)> UpdatePersonProjectsAsync(long personId, List<Project> projects)
        {
            var updated = false;
            var unvoided = false;

            foreach (Project project in projects)
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
                else
                {
                    _logger.LogInformation($"No action taken as person with id {personId} already has access");
                }
            }

            return (updated, unvoided);
        }

        private async Task UpdatePrivlegesAsync(long personId, string plantId, PersonProjectHistory personProjectHistory, List<Project> projects)
        {
            await _privilegeService.GivePrivilegesAsync(plantId, personId);

            projects.ForEach(p =>
            {
                PersonProjectHistoryHelper.LogAddAccess(personId, personProjectHistory, p.ProjectId);

                if (p.IsMainProject)
                {
                    PersonProjectHistoryHelper.LogDefaultUserGroup(personId, personProjectHistory, p.ProjectId);
                }
            });
        }
    }
}