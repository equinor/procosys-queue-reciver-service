using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using QueueReceiver.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.IntegrationTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddSingleton<ILogger<Worker.Worker>,Logger<Worker.Worker>>();
            services.AddServices();
            services.AddRepositories();
            services.AddDbContext("");


            var dbContextSettings = new DbContextSettings();
            //hostContext.Configuration.Bind(nameof(DbContextSettings), dbContextSettings);
            services.AddSingleton(dbContextSettings);

            var graphSettings = new GraphSettings();
           // hostContext.Configuration.Bind(nameof(GraphSettings), graphSettings);
            services.AddSingleton(graphSettings);
            services.AddHostedService<Worker.Worker>();

            services.AddSingleton<IQueueClient>(_ =>
            {
                var connString = "";//configuration["ServiceBusConnectionString"];
                var queueName = "";// configuration["ServiceBusQueueName"];
                var queueClient = new QueueClient(connString, queueName);
                queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
                return queueClient;
            });

            services.AddSingleton<IEntryPointService, EntryPointService>();
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetService<IHostedService>() as Worker.Worker;

            var entryPointService = serviceProvider.GetService<IEntryPointService>() as EntryPointService;

            await service.StartAsync(CancellationToken.None);
            var isExecuted = false;

            await Task.Delay(10000);
            Assert.IsTrue(isExecuted);

            await service.StopAsync(CancellationToken.None);
        }
    }
}
