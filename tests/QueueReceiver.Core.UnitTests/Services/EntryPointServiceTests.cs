using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class EntryPointServiceTests
    {
        private static (EntryPointService,
            TestableQueueClient,
            Mock<ILogger<EntryPointService>>,
            Mock<IAccessService>)
            Factory()
        {
            var logger = new Mock<ILogger<EntryPointService>>();
            var queueClient = new TestableQueueClient();
            var accessService = new Mock<IAccessService>();
            var service = new EntryPointService(queueClient, accessService.Object, logger.Object);

            return (service, queueClient, logger, accessService);
        }

        [TestMethod]
        public async Task InitializeQueueTest()
        {
            var (service, queueClient, logger, accessService) = Factory();

            accessService.Setup(acs => acs.HandleRequest(It.IsAny<AccessInfo>()))
                .ThrowsAsync(new InternalTestFailureException("!"));

            var accessInfo = new AccessInfo("test",
                new List<Member>
                {
                    new Member("testOid",false)
                });

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(accessInfo)));

            await service.InitializeQueue();

            try
            {
                await queueClient.SendMessage(message, default);
            }
            catch (InternalTestFailureException e)
            {
                Assert.AreEqual(e.Message, "!");
            }
        }
    }
}