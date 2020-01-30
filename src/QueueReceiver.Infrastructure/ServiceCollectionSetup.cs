using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
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

        public static void AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<QueueReceiverServiceContext>(options =>
                       {
                           options.UseOracle(connectionString);
                           options.UseLoggerFactory(LoggerFactory);
                       });
            services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<QueueReceiverServiceContext>());
        }

        public static void AddQueueClient(this IServiceCollection services, string serviceBusConnectionString, string serviceBusQueueName)
        {
            services.AddSingleton<IQueueClient>(_ =>
            {
                var queueClient = new QueueClient(serviceBusConnectionString, serviceBusQueueName);
                queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
                return queueClient;
            });
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IPersonProjectRepository, PersonProjectRepository>();
            services.AddScoped<IPlantRepository, PlantRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserGroupRepository, UserGroupRepository>();
            services.AddScoped<IPersonUserGroupRepository, PersonUserGroupRepository>();
            services.AddScoped<IRestrictionRoleRepository, RestrictionRoleRepository>();
            services.AddScoped<IPersonRestrictionRoleRepository, PersonRestrictionRoleRepository>();
            services.AddScoped<IPersonProjectHistoryRepository, PersonProjectHistoryRepository>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IEntryPointService, EntryPointService>();
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IPersonProjectService, PersonProjectService>();
        }
    }
}
