using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueReciverService.Data;
using QueueReciverService.Repositories;
using QueueReciverService.Services;

namespace QueueReciverService
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
            WebProxy proxy = new WebProxy("http://www-proxy.statoil.no:80");
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
                    services.AddScoped<IPersonRepository, PersonRepository>();
                    services.AddScoped<IAccessService, AccessService>();
                    services.AddHostedService<Worker>();
                });
    }
}
