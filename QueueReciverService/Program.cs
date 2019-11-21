using System.IO;
using System.Net;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueReceiverService.Data;
using QueueReceiverService.Repositories;
using QueueReceiverService.Services;

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
            var proxy = new WebProxy("http://www-proxy.statoil.no:80");
            WebRequest.DefaultWebProxy = proxy;

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<ApplicationDbContext>();
                services.AddHostedService<Worker>();

                services.AddSingleton<IQueueClient>(sp =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();
                        var connString = config["ServiceBusConnectionString"];
                        var queueName = "updateuseraccessdev";//config[""];
                    var queueClient = new QueueClient(connString, queueName);
                        queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
                        return queueClient;
                    });

                services.AddTransient<IAccessService, AccessService>();
                services.AddTransient<IPlantService, PlantService>();
                services.AddTransient<IPersonRepository, PersonRepository>();
                services.AddTransient<IGraphService, GraphService>();
                services.AddTransient<IPersonService, PersonService>();
                services.AddTransient<IProjectService, ProjectService>();
                services.AddTransient<IPersonProjectRepository, PersonProjectRepository>();
                services.AddTransient<IPlantRepository, PlantRepository>();
                services.AddTransient<IProjectRepository, ProjectRepository>();
            });
    }
}
