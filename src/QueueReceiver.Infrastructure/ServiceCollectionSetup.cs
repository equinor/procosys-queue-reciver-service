﻿using Microsoft.Azure.ServiceBus;
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
        public static readonly LoggerFactory _myLoggerFactory =
            new LoggerFactory(new[]
            { new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider() });

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration) =>
         services.AddDbContext<ApplicationDbContext>(options =>
         {
             options.UseOracle(configuration["ConnectionString"]);
             options.UseLoggerFactory(_myLoggerFactory);
         });

        public static void AddQueueClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IQueueClient>(_ =>
            {
                var connString = configuration["ServiceBusConnectionString"];
                var queueName = "updateuseraccessdev";//config[""];
                var queueClient = new QueueClient(connString, queueName);
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
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccessService, AccessService>();
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IProjectService, ProjectService>();
        }
    }
}
