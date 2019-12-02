﻿using Microsoft.EntityFrameworkCore;
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
        private readonly DbContextSettings _settings;

        public PersonProjectRepository(ApplicationDbContext context, DbContextSettings settings)
        {
            _personProjects = context.Personprojects;
            _context = context;
            _settings = settings;
        }

        public async Task AddAsync(long projectId, long personId)
        {
            var createdById = _settings.PersonProjectCreatedId;
            var personProject = new PersonProject(projectId, personId, createdById);
            await _personProjects.AddAsync(personProject);
        }

        public void VoidPersonProjects(string plantId, long personId)
        {
            var personProjects = _personProjects
                .Include(pp => pp.Project!)
                .ThenInclude(project => project.Plant)
                .Where(pp => plantId.Equals(pp.Project!.PlantId)
                    && personId == pp.PersonId);

            personProjects.ForEachAsync(pp => pp.IsVoided = true);
            _personProjects.UpdateRange(personProjects);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<PersonProject> GetAsync(long projectId, long personId)
        {
            return await _personProjects.FindAsync(projectId, personId);
        }

        public void Update(PersonProject personProject)
        {
            _personProjects.Update(personProject);
        }
    }
}