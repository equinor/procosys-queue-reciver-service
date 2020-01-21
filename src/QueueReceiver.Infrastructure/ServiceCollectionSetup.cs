using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Infrastructure.EntityConfiguration;
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
            => services.AddDbContext<QueueReceiverServiceContext>(options =>
                {
                    options.UseOracle(configuration["ConnectionString"]);
                    options.UseLoggerFactory(LoggerFactory);
                }).AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<QueueReceiverServiceContext>());

        public static void AddQueueClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IQueueClient>(_ =>
            {
                var connString = configuration["ServiceBusConnectionString"];
                var queueName = configuration["ServiceBusQueueName"];
                var queueClient = new QueueClient(connString, queueName);
                queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
                return queueClient;
            });
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
            => services.AddScoped<IPersonRepository, PersonRepository>()
            .AddScoped<IPersonProjectRepository, PersonProjectRepository>()
            .AddScoped<IPlantRepository, PlantRepository>()
            .AddScoped<IProjectRepository, ProjectRepository>()
            .AddScoped<IUserGroupRepository, UserGroupRepository>()
            .AddScoped<IPersonUserGroupRepository, PersonUserGroupRepository>()
            .AddScoped<IRestrictionRoleRepository, RestrictionRoleRepository>()
            .AddScoped<IPersonRestrictionRoleRepository, PersonRestrictionRoleRepository>()
            .AddScoped<IPersonProjectHistoryRepository, PersonProjectHistoryRepository>();
        

        public static IServiceCollection AddServices(this IServiceCollection services)
            => services.AddScoped<IAccessService, AccessService>()
            .AddScoped<IPlantService, PlantService>()
            .AddScoped<IGraphService, GraphService>()
            .AddScoped<IPersonService, PersonService>()
            .AddScoped<IPersonProjectService, PersonProjectService>()
            .AddScoped<IPersonProjectHistoryService, PersonProjectHistoryService>();
    }
}
