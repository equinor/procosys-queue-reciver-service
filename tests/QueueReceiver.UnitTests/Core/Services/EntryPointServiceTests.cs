using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueueReceiver.UnitTests.Core.Services
{
    [TestClass()] //TODO this class might get rewritten and be better for testing
    public class EntryPointServiceTests 
    {
        private static (EntryPointService,
            TestableQueueClient,
            Mock<IServiceLocator>,
            Mock<ILogger<EntryPointService>>,
            Mock<IAccessService>)
            Factory()
        {
            var logger = new Mock<ILogger<EntryPointService>>();
            var queueClient = new TestableQueueClient();
            var serviceLocator = new Mock<IServiceLocator>();
            var service = new EntryPointService(queueClient, serviceLocator.Object, logger.Object);

            var accessService = SetupCreateScope(serviceLocator);

            return (service, queueClient, serviceLocator, logger, accessService);
        }

        private static Mock<IAccessService> SetupCreateScope(Mock<IServiceLocator> serviceLocator)
        {
            var fakeScope = new Mock<IServiceScope>();
            serviceLocator.Setup(sl => sl.CreateScope())
                            .Returns(fakeScope.Object);

            var serviceProvider = new Mock<IServiceProvider>();
            fakeScope.Setup(s => s.ServiceProvider)
                .Returns(serviceProvider.Object);

            var service = new Mock<IAccessService>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IAccessService)))
                .Returns(service.Object);

            return service;
        }

        [TestMethod]
        public async Task InitializeQueueTest()
        {
            var (service, queueClient, _, logger, accessService) = Factory();

            accessService.Setup(acs => acs.HandleRequest(It.IsAny<AccessInfo>()))
                .ThrowsAsync(new Exception("!"));

            var accessInfo = new AccessInfo("test",
                new List<Member>
                {
                    new Member("testOid",false)
                });

            Mock<Message> message = new Mock<Message>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(accessInfo)));

            await service.InitializeQueue();

            try
            {
                await queueClient.SendMessage(message.Object, default);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "!");
            }
        }
    }
}