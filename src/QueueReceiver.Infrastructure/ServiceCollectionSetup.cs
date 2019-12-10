using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Infrastructure.Data;
using QueueReceiver.Infrastructure.Repositories;

namespace QueueReceiver.Infrastructure
{
    public static class ServiceCollectionSetup
    {
        public static readonly LoggerFactory LoggerFactory =
            new LoggerFactory(new[]
            {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseOracle(configuration["ConnectionString"]);
                options.UseLoggerFactory(LoggerFactory);
            });

        public static void AddQueueClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IQueueClient>(_ =>
            {
                var connString = configuration["ServiceBusConnectionString"];
                var queueName = "updateuseraccessdev";//TODO config[""];
                var queueClient = new QueueClient(connString, queueName);
                queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
                return queueClient;
            });
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IPersonProjectRepository, PersonProjectRepository>();
            services.AddScoped<IPlantRepository, PlantRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserGroupRepository, UserGroupRepository>();
            services.AddScoped<IPersonUserGroupRepository, PersonUserGroupRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IProjectService, ProjectService>();
            return services;
        }
    }
}
