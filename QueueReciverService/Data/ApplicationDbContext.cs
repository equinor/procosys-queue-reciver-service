using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QueueReceiverService.Models;

namespace QueueReceiverService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public static readonly LoggerFactory _myLoggerFactory =
        new LoggerFactory(new[] {
            new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
        });

        public IConfiguration Configuration { get; }

        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonProject> Personprojects { get; set; }
        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Plant> Plants { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLoggerFactory(_myLoggerFactory);
                string connectionString = Configuration["ConnectionString"];
                optionsBuilder.UseOracle(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonProject>()
                .HasKey(pp => new { pp.ProjectId, pp.PersonId });
            modelBuilder.Entity<PersonProject>()
                .HasOne(pp=> pp.Project)
                .WithMany()
                .HasForeignKey(pp=> pp.ProjectId);

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
