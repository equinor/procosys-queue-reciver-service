using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Services.CleanArchitecture.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using QueueReceiver.Infrastructure.Data;
using System.IO;
using System.Net;

namespace QueueReceiverService
{
    public class Program
    {
        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static void Main(string[] args)
        {
            WebRequest.DefaultWebProxy = new WebProxy("http://www-proxy.statoil.no:80"); //TODO move this to infrastructure and add as variable
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IEntryPointService, EntryPointService>();
                services.AddSingleton<IServiceLocator, ServiceLocator>();

                services.AddDbContext(hostContext.Configuration);
                services.AddQueueClient(hostContext.Configuration);
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
