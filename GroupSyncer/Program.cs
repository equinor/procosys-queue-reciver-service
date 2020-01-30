using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using QueueReceiver.Infrastructure.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GroupSyncer
{
   public class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("hosting.json", false, true)
            .AddUserSecrets<Program>()
            .Build();

            var dbContextSettings = new DbContextSettings();
            config.Bind(nameof(DbContextSettings), dbContextSettings);
            var graphSettings = new GraphSettings();
            config.Bind(nameof(GraphSettings), graphSettings);

            //setup our DI
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ISyncService, SyncService>()
                .AddSingleton(dbContextSettings)
                .AddSingleton(graphSettings)
                .AddServices()
                .AddRepositories()
                .AddDbContext(config["ConnectionString"])
                .BuildServiceProvider();

            var syncService = services.GetService<ISyncService>();
            Console.WriteLine("starting sync");
            await syncService.ExcecuteOidSync();
            Console.WriteLine("Sync Done!");

        }
    }
}
