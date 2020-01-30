using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using QueueReceiver.Infrastructure.Data;

namespace QueueReceiver.Worker
{
    public class Program
    {
        public Program(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public static void Main(string[] args)
        {
            WebRequest.DefaultWebProxy = new WebProxy("http://www-proxy.statoil.no:80"); //TODO move this to infrastructure and add as variable.
            CreateHostBuilder(args).Build().Run(); //TODO: Split this between Build() and Run() and get configuration in between and use that to set the proxy.
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureAppConfiguration((_, config) =>
            {
                config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseContentRoot(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext(hostContext.Configuration["ConnectionString"]);
                services.AddQueueClient(
                    hostContext.Configuration["ServiceBusConnectionString"],
                    hostContext.Configuration["ServiceBusQueueName"]);
                services.AddRepositories();
                services.AddServices();

                var dbContextSettings = new DbContextSettings();
                hostContext.Configuration.Bind(nameof(DbContextSettings), dbContextSettings);
                services.AddSingleton(dbContextSettings);

                var graphSettings = new GraphSettings();
                hostContext.Configuration.Bind(nameof(GraphSettings), graphSettings);
                services.AddSingleton(graphSettings);

                services.AddHostedService<Worker>();
            });
    }
}
