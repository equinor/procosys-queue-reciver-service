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

                services.AddScoped<IAccessService, AccessService>();
                services.AddScoped<IPlantService, PlantService>();
                services.AddScoped<IPersonRepository, PersonRepository>();
                services.AddScoped<IGraphService, GraphService>();
                services.AddScoped<IPersonService, PersonService>();
                services.AddScoped<IProjectService, ProjectService>();
                services.AddScoped<IPersonProjectRepository, PersonProjectRepository>();
                services.AddScoped<IPlantRepository, PlantRepository>();
                services.AddScoped<IProjectRepository, ProjectRepository>();
            });
    }
}
