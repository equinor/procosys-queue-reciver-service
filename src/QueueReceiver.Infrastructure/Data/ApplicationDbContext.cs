using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Threading.Tasks;
using Person = QueueReceiver.Core.Models.Person;

namespace QueueReceiver.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){}

        public ApplicationDbContext()
        {
        }

        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<PersonProject> PersonProjects { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<Plant> Plants { get; set; } = null!;
        public virtual DbSet<PersonUserGroup> PersonUserGroups { get; set; } = null!;
        public virtual DbSet<PersonRestrictionRole> PersonRestrictionRoles { get; set; } = null!;
        public virtual DbSet<PersonProjectHistory> PersonProjectHistories { get; set; } = null!;
        public virtual DbSet<PersonProjectHistoryOperation> PersonProjectHistoryOperations { get; set; } = null!;
        public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;
        public virtual DbSet<RestrictionRole> RestrictionRoles { get; set; } = null!;

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PersonProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new PersonUserGroupConfiguration());
            modelBuilder.ApplyConfiguration(new RestrictionRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PersonRestrictionRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PersonProjectHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new PersonProjectHistoryOperationConfiguration());
        }

  
    }
}
