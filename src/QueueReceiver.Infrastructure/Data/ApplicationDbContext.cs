using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){}

        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<PersonProject> Personprojects { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Plant> Plants { get; set; } = null!;
        public virtual DbSet<PersonUserGroup> PersonUserGroups { get; set; } = null!;
        public virtual DbSet<PersonProjectHistory> PersonProjectHistories { get; set; } = null!;
        public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PersonProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new PersonUserGroupConfiguration());
        }
    }
}
