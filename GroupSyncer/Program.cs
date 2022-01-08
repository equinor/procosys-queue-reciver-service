using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
                Console.WriteLine(@"Error: Invalid number of arguments.");
                return;
            }

            if (args.Length == 1)
            {
                var argPlants = args[0].Split(',');

                foreach (var plant in argPlants)
                {
                    if (!plant.StartsWith("PCS$"))
                    {
                        Console.WriteLine(@"Error: Plant names must start with 'PCS$'");
                        return;
                    }

                    plants.Add(plant);
                }
            }

            var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"{path}\\hosting.json", false, true)
                .AddUserSecrets<Program>()
                .Build();

            var removeUserAccess = bool.Parse(config["RemoveUserAccessEnabled"]);

            var personCreatedById = long.Parse(config["PersonCreatedById"], CultureInfo.InvariantCulture);
            var personCreatedByCache = new PersonCreatedByCache(personCreatedById);

            var graphSettings = new GraphSettings();
            config.Bind(nameof(GraphSettings), graphSettings);

            //setup our DI
            var serviceCollection = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .AddApplicationInsightsTelemetryWorkerService(config["ApplicationInsights:InstrumentationKey"])
                .AddSingleton<ISyncService, SyncService>()
                .AddSingleton(personCreatedByCache)
                .AddSingleton(graphSettings)
                .AddServices()
                .AddRepositories()
                .AddDbContext(config["ConnectionString"]);

            var services = serviceCollection.BuildServiceProvider();

            var logger = services.GetRequiredService<ILogger<Program>>();

            var syncService = services.GetService<ISyncService>();

            try
            {
                await syncService.StartAccessSync(plants, removeUserAccess);
            }
            catch (Exception e)
            {
                logger.LogError($"[GroupSync Error] : {e.Message}");
                logger.LogError(e.StackTrace);
            }

            // Take a break to allow AI to finish logging.
            await Task.Delay(10000);
        }
    }
}
