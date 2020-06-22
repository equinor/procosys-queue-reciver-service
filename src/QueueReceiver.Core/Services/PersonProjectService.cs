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
        private readonly PersonCreatedByCache _personCreatedByCache;

        public PersonProjectService(
            IPersonProjectRepository personProjectRepository,
            IProjectRepository projectRepository,
            IPrivilegeService privilegeService,
            IPersonProjectHistoryRepository personProjectHistoryRepository,
            IPersonService personService,
            ILogger<PersonProjectService> logger, 
            PersonCreatedByCache personCreatedByCache)
        {
            _personProjectRepository = personProjectRepository;
            _projectRepository = projectRepository;
            _privilegeService = privilegeService;
            _personProjectHistoryRepository = personProjectHistoryRepository;
            _personService = personService;
            _logger = logger;
            _personCreatedByCache = personCreatedByCache;
        }

        public async Task<bool> GiveProjectAccessToPlantAsync(long personId, string plantId)
        {
            var hasChanges = false;

            var personProjectHistory =
                PersonProjectHistoryHelper.CreatePersonProjectHistory(
                    _personCreatedByCache.Id,
                    _personCreatedByCache.Username);

            var projects = await _projectRepository.GetParentProjectsByPlant(plantId);

            var (updatedProjectIds, unvoidedProjectIds) = await UpdatePersonProjectsAsync(personId, projects);

            if (updatedProjectIds.Any())
            {
                await UpdatePrivlegesAsync(personId, plantId, personProjectHistory, projects, updatedProjectIds);
            }

            if (unvoidedProjectIds.Any())
            {
                unvoidedProjectIds.ForEach(projectId =>
                    PersonProjectHistoryHelper.LogUnvoidProjects(
                        personId,
                        personProjectHistory,
                        projectId,
                        _personCreatedByCache.Username));
            }

            if (updatedProjectIds.Any() || unvoidedProjectIds.Any())
            {
                await _personProjectHistoryRepository.AddAsync(personProjectHistory);

                hasChanges = true;
            }

            return hasChanges;
        }

        public async Task<bool> RemoveAccessToPlant(long personId, string plantId)
        {
            var hasChanges = false;

            var personProjectHistory =
                PersonProjectHistoryHelper.CreatePersonProjectHistory(
                    _personCreatedByCache.Id,
                    _personCreatedByCache.Username);

            var projects = _personProjectRepository.VoidPersonProjects(plantId, personId).Select(pp => pp.Project!).ToList();
            
            projects.ForEach(p => PersonProjectHistoryHelper.LogVoidProjects(
                personId,
                personProjectHistory,
                p.ProjectId,
                _personCreatedByCache.Username));

            if (projects.Count > 0)
            {
                await _personProjectHistoryRepository.AddAsync(personProjectHistory);

                hasChanges = true;
            }
            else
            {
                _logger.LogInformation(
                    $"Access to all projects are already voided for person {personId} and plant {plantId}. No action taken.");
            }

            return hasChanges;
        }

        private async Task<(List<long> updated, List<long> unvoided)> UpdatePersonProjectsAsync(long personId, List<Project> projects)
        {
            var updatedProjectIds = new List<long>();
            var unvoidedProjectIds = new List<long>();

            foreach (Project project in projects)
            {
                var projectId = project.ProjectId;
                var personProject = await _personProjectRepository.GetAsync(projectId, personId);

                if (personProject == null)
                {
                    await _personProjectRepository.AddAsync(projectId, personId, _personCreatedByCache.Id);
                    updatedProjectIds.Add(projectId);
                }
                else if (personProject.IsVoided)
                {
                    personProject.IsVoided = false;
                    unvoidedProjectIds.Add(projectId);
                }
                else
                {
                    _logger.LogInformation($"Person id {personId} already has access to project id {project.ProjectId}. No action taken.");
                }
            }

            return (updatedProjectIds, unvoidedProjectIds);
        }

        private async Task UpdatePrivlegesAsync(long personId, 
            string plantId,
            PersonProjectHistory personProjectHistory, 
            List<Project> parentProjects, 
            List<long> updatedProjectIds)
        {
            await _privilegeService.GivePrivilegesAsync(plantId, personId);

            updatedProjectIds.ForEach(projectId =>
            {
                PersonProjectHistoryHelper.LogAddAccess(
                    personId,
                    personProjectHistory,
                    projectId,
                    _personCreatedByCache.Username);

                var isMainProject = parentProjects.Single(p => p.ProjectId == projectId).IsMainProject;

                if (isMainProject)
                {
                    PersonProjectHistoryHelper.LogDefaultUserGroup(
                        personId,
                        personProjectHistory,
                        projectId,
                        _personCreatedByCache.Username);
                }
            });
        }
    }
}