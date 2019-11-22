using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Repositories
{
    public class PersonProjectRepository : IPersonProjectRepository
    {
        private readonly DbSet<PersonProject> _personProjects;
        private readonly ApplicationDbContext _context;

        public PersonProjectRepository(ApplicationDbContext context)
        {
            _personProjects = context.Personprojects;
            _context = context;
        }

        public async Task AddIfNotExists(long personId, long projectId)
        {
            var personProject = _personProjects.Find(personId, projectId);

            if (personProject == null)
            {
                await _personProjects.AddAsync(new PersonProject(personId, projectId));
            }
        }

        public void RemovePersonProjects(string plantId, long personId)
        {
            var personProjects = _personProjects
                .Include(pp => pp.Project!)
                .ThenInclude(project => project.Plant)
                .Where(pp => pp.Project != null
                    && plantId.Equals(pp.Project.PlantId)
                    && personId == pp.PersonId);

            var toSee = personProjects.ToList();

            _personProjects.RemoveRange(personProjects);
        }

        public void RemoveIfExists(long personId, long projectId)
        {
            var personProject = _personProjects.Find(personId, projectId);
            if (personProject != null)
            {
                _personProjects.Remove(personProject);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
