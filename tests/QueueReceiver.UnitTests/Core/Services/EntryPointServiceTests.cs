using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using System;
using System.Threading.Tasks;

namespace QueueReceiver.UnitTests.Core.Services
{
    [TestClass()]
    public class EntryPointServiceTests
    {
        private static (EntryPointService, Mock<IQueueClient>, Mock<IServiceLocator>, Mock<ILogger<EntryPointService>>, Mock<IAccessService>) Factory()
        {
            var logger = new Mock<ILogger<EntryPointService>>();
            var accessService = new Mock<IAccessService>();
            var queueClient = new Mock<IQueueClient>();
            var serviceLocator = new Mock<IServiceLocator>();
            var service = new EntryPointService(queueClient.Object, serviceLocator.Object, logger.Object);

            return (service, queueClient, serviceLocator, logger, accessService);
        }

        [TestMethod()]
        public async Task EntryPointServiceTest()
        {
            var (service, _, _, logger, accessService) = Factory();

            accessService.Setup(acs => acs.HandleRequest(It.IsAny<AccessInfo>()))
                .ThrowsAsync(new Exception("!"));

            await service.InitializeQueue();

        }

        [TestMethod()]
        public void InitializeQueueTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DisposeQueueTest()
        {
            Assert.Fail();
        }
    }
}