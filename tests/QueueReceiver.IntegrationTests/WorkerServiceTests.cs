using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using QueueReceiver.Worker;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.IntegrationTests
{
    [TestClass]
    public class WorkerServiceTests
    {
        private const int MillisecondsDelay = 1000;

        private IConfiguration Configuration { get;  }

        public WorkerServiceTests()
        {
            WebRequest.DefaultWebProxy = new WebProxy("http://www-proxy.statoil.no:80");
            var builder = new ConfigurationBuilder()
                 .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                 .AddJsonFile("appsettings.integrationtests.json", optional: false, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .AddUserSecrets<WorkerServiceTests>();

            Configuration = builder.Build();
        }

        #region Facory Methods
        private (WorkerService, QueueClient, Mock<IAccessService>) Factory()
        {
            var queueClient = CreateQueueClient();
            var accessServiceMock = new Mock<IAccessService>();
            var serviceLocatorMock = new Mock<IServiceLocator>();
            var scopeMock = new Mock<IServiceScope>();
            serviceLocatorMock.Setup(sp => sp.CreateScope()).Returns(scopeMock.Object);
            var serviceProviderMock = new Mock<IServiceProvider>();
            scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);
            var EntryServiceLogger = new Mock<ILogger<EntryPointService>>();
            var entryPointService = new EntryPointService(queueClient, accessServiceMock.Object, EntryServiceLogger.Object);
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IEntryPointService)))
                .Returns(entryPointService);

            var workerServiceLogger = new Mock<ILogger<WorkerService>>();
            var workerService = new WorkerService(workerServiceLogger.Object, serviceLocatorMock.Object);

            return (workerService, queueClient, accessServiceMock);
        }

        private QueueClient CreateQueueClient()
        {
            string connectionString = Configuration["ServiceBusConnectionString"];
            var queueClient = new QueueClient(connectionString, "intergrationtest");
            queueClient.ServiceBusConnection.TransportType = TransportType.AmqpWebSockets;
            return queueClient;
        }
        #endregion

        [TestMethod]
        public async Task EntryPointService_HandlesMessagesCorrectly_UsingRealQueue()
        {
            //Arrange
            var (workerService, queueClient, accessServiceMock) = Factory();

            //Act
            await workerService.StartAsync(CancellationToken.None);
            var testMessage = CreateTestMessage();
            await queueClient.SendAsync(testMessage);
            await Task.Delay(MillisecondsDelay);

            //Assert
            accessServiceMock.Verify(acs => acs.HandleRequestAsync(It.IsAny<AccessInfo>()), Times.Once);

            await workerService.StopAsync(CancellationToken.None);
        }

        private Message CreateTestMessage()
        {
            const string userOid = "testOid";
            const bool shouldRemove = false;
            const string plantOid = "testId";
            var json = JObject.FromObject(new
            {
                groupId = plantOid,
                members = new object[] { new { id = userOid, remove = shouldRemove } }
            }).ToString();

            return new Message(Encoding.UTF8.GetBytes(json));
        }
    }
}
