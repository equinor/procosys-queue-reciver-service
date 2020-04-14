using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonProjectRepository : IPersonProjectRepository
    {
        private readonly DbSet<PersonProject> _personProjects;

        public PersonProjectRepository(QueueReceiverServiceContext context)
        {
            _personProjects = context.PersonProjects;
        }

        public async Task AddAsync(long projectId, long personId, long createdById)
        {
            var personProject = new PersonProject(projectId, personId, createdById);
            await _personProjects.AddAsync(personProject);
        }

        public List<PersonProject> VoidPersonProjects(string plantId, long personId)
        {
            var personProjects = _personProjects
                .Include(pp => pp.Project!)
                .ThenInclude(project => project.Plant)
                .Where(pp => plantId.Equals(pp.Project!.PlantId)
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

        public async Task<bool> PersonHasNoAccess(long personId)
        {
            return !await _personProjects.AnyAsync(pp => pp.PersonId == personId
                                                         && pp.IsVoided == false);
        }
    }
}