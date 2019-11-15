﻿using Microsoft.EntityFrameworkCore;
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
        public virtual DbSet<Personproject> Personproject { get; set; }

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
            modelBuilder.Entity<Personproject>().HasKey(pp => new { pp.ProjectId, pp.PersonId });
        }

    }
}
