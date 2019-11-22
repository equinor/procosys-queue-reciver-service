using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DbContextSettings settings;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            DbContextSettings dbContextSettings
        ): base(options)
        {
            settings = dbContextSettings;
        }

        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<PersonProject> Personprojects { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;

        public virtual DbSet<Plant> Plants { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PersonProject>()
                .Property(pp => pp.CreatedById)
                .HasDefaultValue(settings.PersonProjectCreatedId);

            modelBuilder.Entity<PersonProject>()
                .HasKey(pp => new { pp.ProjectId, pp.PersonId });
            modelBuilder.Entity<PersonProject>()
                .HasOne(pp => pp.Project)
                .WithMany()
                .HasForeignKey(pp => pp.ProjectId);

            modelBuilder.Entity<Project>()
                .HasOne(project => project.Plant)
                .WithMany();

            modelBuilder.Entity<Project>()
            .Property(p => p.IsVoided)
            .HasConversion(
                b => b ? 'Y' : 'N',
                c => c.Equals('Y'));
        }


    }
}
