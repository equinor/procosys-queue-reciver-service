using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using System;
using System.Globalization;
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

            var personCreatedById = long.Parse(config["PersonCreatedById"], CultureInfo.InvariantCulture);
            var personCreatedByCache = new PersonCreatedByCache(personCreatedById);

            var graphSettings = new GraphSettings();
            config.Bind(nameof(GraphSettings), graphSettings);

            //setup our DI
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ISyncService, SyncService>()
                .AddSingleton(personCreatedByCache)
                .AddSingleton(graphSettings)
                .AddServices()
                .AddRepositories()
                .AddDbContext(config["ConnectionString"])
                .BuildServiceProvider();

            var syncService = services.GetService<ISyncService>();
            Console.WriteLine("Starting sync");
            await syncService.StartAccessSync();
            Console.WriteLine("Sync Done!");

        }
    }
}
