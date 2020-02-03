using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using QueueReceiver.Core.Settings;
using QueueReceiver.Infrastructure;
using QueueReceiver.Infrastructure.Data;
using QueueReceiver.Worker;
using System;
using System.Collections.Generic;
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
            var plantOid = "";
            var plantId = "";
            var accessInfo = new AccessInfo(plantOid, new List<Member>());
            //var userGroupRepository = new Mock<IUserGroupRepository>();
            //var personUserGroupRepository = new Mock<IPersonUserGroupRepository>();
            //var projectRepository = new Mock<IProjectRepository>();
            //var restrictionRoleRepository = new Mock<IRestrictionRoleRepository>();
            //var personProjectHistoryRepository = new Mock<IPersonProjectHistoryRepository>();
            //var personProjectRepository = new Mock<IPersonProjectRepository>();
            //var personRestrictionRoleRepository = new Mock<IPersonRestrictionRoleRepository>();
            //var personRepository = new Mock<IPersonRepository>();
            //var unitOfWork = new Mock<IUnitOfWork>();

            //var accessServiceLogger = new Mock<ILogger<AccessService>>();
            var EntryServiceLogger = new Mock<ILogger<EntryPointService>>();

            //var plantRepository = new Mock<IPlantRepository>();
            //plantRepository.Setup(pr => pr.GetPlantIdByOid(plantOid)).Returns(Task.FromResult(plantId));
            //var plantService = new PlantService(plantRepository.Object);

            //var graphService = new Mock<IGraphService>();

            //var personService = new PersonService(personRepository.Object, graphService.Object);

            //var personProjectService = new PersonProjectService(personProjectRepository.Object, projectRepository.Object,
            //    personUserGroupRepository.Object, userGroupRepository.Object,
            //    personRestrictionRoleRepository.Object, restrictionRoleRepository.Object,
            //    personProjectHistoryRepository.Object);

            var queueClientMock = new Mock<IQueueClient>(); //TODO
            var accessServiceMock = new Mock<IAccessService>();// (personService, personProjectService, plantService, accessServiceLogger.Object, unitOfWork.Object);

            var serviceProvider = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<WorkerService>>();
            var serviceScope = new Mock<IServiceScope>();

            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            serviceProvider.Setup(sp => sp.CreateScope()).Returns(serviceScope.Object);

            serviceProvider.Setup(ss => ss.GetService(typeof(IEntryPointService)))
                .Returns(new EntryPointService(queueClientMock.Object, accessServiceMock.Object, EntryServiceLogger.Object));

            var workerService = new WorkerService(logger.Object,serviceProvider.Object);
            //var service = serviceProvider.GetService<IHostedService>() as Worker.Worker;

          //  var entryPointService = serviceProvider.GetService<IEntryPointService>() as EntryPointService;

            await workerService.StartAsync(CancellationToken.None);
            var isExecuted = false;

            await Task.Delay(10000);
            Assert.IsTrue(isExecuted);

            accessServiceMock.Verify(acs => acs.HandleRequest(accessInfo), Times.Once);

            await workerService.StopAsync(CancellationToken.None);
        }
    }
}
