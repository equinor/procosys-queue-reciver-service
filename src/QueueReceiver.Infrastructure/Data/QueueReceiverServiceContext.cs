using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.EntityConfigurations;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.Data
{
    public class QueueReceiverServiceContext : DbContext, IUnitOfWork
    {
        public QueueReceiverServiceContext(DbContextOptions<QueueReceiverServiceContext> options)
            : base(options) { }

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
            modelBuilder.ApplyConfiguration(new PersonConfiguration())
                .ApplyConfiguration(new PersonProjectConfiguration())
                .ApplyConfiguration(new PersonUserGroupConfiguration())
                .ApplyConfiguration(new PersonRestrictionRoleConfiguration())
                .ApplyConfiguration(new PersonProjectHistoryConfiguration())
                .ApplyConfiguration(new ProjectConfiguration())
                .ApplyConfiguration(new PlantConfiguration())
                .ApplyConfiguration(new PersonProjectHistoryOperationConfiguration())
                .ApplyConfiguration(new RestrictionRoleConfiguration())
                .ApplyConfiguration(new UserGroupConfiguration());
        }
    }
}
