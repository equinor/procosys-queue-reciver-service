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
        /**
         * Maximum open cursors in the Pcs database is configured to 300 as per 05.03.2020.
         * When doing batch updates/inserts, oracle opens a cursor per update/insert to keep track of
         * the amount of entities updated. The default seems to be 200, but we're setting it explicitly anyway
         * in case the default changes in the future. This is to avoid ORA-01000: maximum open cursors exceeded.
         **/
        private const int MAX_OPEN_CURSORS = 200;

        public static readonly LoggerFactory LoggerFactory =
            new LoggerFactory(new[]
            {
                new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
            });

        public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
            => services.AddDbContext<QueueReceiverServiceContext>(options =>
                {
                    options.UseOracle(connectionString, b => b.MaxBatchSize(MAX_OPEN_CURSORS));
                    options.UseLoggerFactory(LoggerFactory);
                    options.EnableSensitiveDataLogging();
                }).AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<QueueReceiverServiceContext>());

        public static void AddQueueClient(this IServiceCollection services, string serviceBusConnectionString, string serviceBusQueueName)
        {
            services.AddSingleton<IQueueClient>(_ =>
            {
                var queueClient = new QueueClient(serviceBusConnectionString, serviceBusQueueName);
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
            => services.AddSingleton<IServiceLocator, ServiceLocator>()
            .AddSingleton<IEntryPointService, EntryPointService>()
            .AddScoped<IAccessService, AccessService>()
            .AddScoped<IPlantService, PlantService>()
            .AddScoped<IGraphService, GraphService>()
            .AddScoped<IPersonService, PersonService>()
            .AddScoped<IPersonProjectService, PersonProjectService>()
            .AddScoped<IPrivilegeService,PrivilegeService>();
    }
}
