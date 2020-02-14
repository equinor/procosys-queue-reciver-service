using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using QueueReceiver.Infrastructure.Data;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonProjectRepository : IPersonProjectRepository
    {
        private readonly DbSet<PersonProject> _personProjects;
        private readonly DbContextSettings _settings;

        public PersonProjectRepository(QueueReceiverServiceContext context, DbContextSettings settings)
        {
            _personProjects = context.PersonProjects;
            _settings = settings;
        }

        public async Task AddAsync(long projectId, long personId)
        {
            var createdById = _settings.PersonProjectCreatedId;
            var personProject = new PersonProject(projectId, personId, createdById);
            await _personProjects.AddAsync(personProject);
        }

        public List<PersonProject> VoidPersonProjects(string plantId, long personId)
        {
            var personProjects = _personProjects
                .Include(pp => pp.Project!)
                .ThenInclude(project => project.Plant)
                .Where(pp => plantId.Equals(pp.Project!.PlantId, StringComparison.Ordinal)
                             && personId == pp.PersonId);
            personProjects.ForEachAsync(pp => pp.IsVoided = true);

            return personProjects.ToList();
        }

        public async Task<PersonProject> GetAsync(long projectId, long personId)
            => await _personProjects.FindAsync(projectId, personId);

        public IEnumerable<PersonProject> GetByProject(long projectId)
        {
            return _personProjects.Where(pp => pp.ProjectId == projectId);
        }
    }
}