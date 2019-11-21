using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QueueReceiverService.Data;
using QueueReceiverService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiverService.Repositories
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

        public async Task<int> AddIfNotExists(long personId, long projectId)
        {
            var personProject = _personProjects.Find(personId, projectId);
            int dbOpperations = 0;

            if (personProject == null)
            {
                await _personProjects.AddAsync(new PersonProject(personId, projectId));
                dbOpperations++;
            }
            return dbOpperations;

        }

        public Task<List<PersonProject>> GetPersonProjects(string plantId, int personId)
        {
            return _personProjects
                .Where(pp => plantId.Equals(pp.Project.PlantId) && personId == pp.PersonId)
                .ToListAsync();
        }

        public int RemoveIfExists(long personId, long projectId)
        {
            var personProject = _personProjects.Find(personId, projectId);
            int dbOpperations = 0;
            if (personProject != null)
            {
                _personProjects.Remove(personProject);
                dbOpperations++;
            }
            return dbOpperations;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
