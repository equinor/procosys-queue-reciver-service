using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GroupSyncer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var plants = new List<string>();

            if (args.Length > 1)
            {
                Console.WriteLine("Error: Invalid number of arguments.");
                return;
            }

            if (args.Length == 1)
            {
                var argPlants = args[0].Split(',');

                foreach (var plant in argPlants)
                {
                    if (!plant.StartsWith("PCS$"))
                    {
                        Console.WriteLine("Error: Plant names must start with 'PCS$'");
                        return;
                    }

                    plants.Add(plant);
                }
            }

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

            try
            {
                Console.WriteLine("Starting Sync.");
                await syncService.StartAccessSync(plants);
                Console.WriteLine("Sync Done!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
