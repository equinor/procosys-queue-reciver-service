using System.Net;
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
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<Program>();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>();
                    services.AddTransient<IPersonRepository, PersonRepository>();
                    services.AddTransient<IProjectService, ProjectService>();
                    services.AddScoped<IAccessService, AccessService>();
                    services.AddHostedService<Worker>();
                });
    }
}
