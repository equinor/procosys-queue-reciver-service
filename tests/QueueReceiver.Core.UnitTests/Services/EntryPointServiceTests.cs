﻿using Microsoft.Azure.ServiceBus;
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
using System.Threading.Tasks;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class EntryPointServiceTests
    {
        private static (EntryPointService,
            TestableQueueClient,
            Mock<IAccessService>,
            Mock<IServiceLocator> serviceLocator)
            Factory()
        {
            var logger = new Mock<ILogger<EntryPointService>>();
            var queueClient = new TestableQueueClient();
            var serviceLocator = new Mock<IServiceLocator>();
            var accessService = new Mock<IAccessService>();
            var service = new EntryPointService(queueClient, serviceLocator.Object, logger.Object);

            return (service, queueClient, accessService, serviceLocator);
        }

        [TestMethod]
        public async Task InitializeQueueTest()
        {
            var (service, queueClient, accessServiceMock, serviceLocator) = Factory();

            var scope = new Mock<IServiceScope>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IAccessService)))
                .Returns(accessServiceMock.Object);
            scope.Setup(sc => sc.ServiceProvider).Returns(serviceProviderMock.Object);

            serviceLocator.Setup(sl => sl.GetService<IAccessService>()).Returns(accessServiceMock.Object);
            accessServiceMock.Setup(acs => acs.HandleRequestAsync(It.IsAny<AccessInfo>()))
                .ThrowsAsync(new InternalTestFailureException("!"));

            var accessInfo = new AccessInfo("test",
                new List<Member>
                {
                    new Member("testOid",false)
                });

            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(accessInfo)));

            await service.InitializeQueueAsync();

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