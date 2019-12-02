using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using System.Threading.Tasks;

namespace QueueReceiver.UnitTests.Core.Services
{
    [TestClass()]
    public class PersonProjectServiceTests
    {

        private static (ProjectService,
           Mock<IPersonProjectRepository>,
           Mock<IProjectRepository>,
           Mock<IPersonUserGroupRepository>,
           Mock<IUserGroupRepository>)
           Factory()
        {
            //var logger = new Mock<ILogger<ProjectService>>();
            var personProjectRepository = new Mock<IPersonProjectRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var personUserGroupRepository = new Mock<IPersonUserGroupRepository>();
            var userGroupRepository = new Mock<IUserGroupRepository>();
            var service = new ProjectService(
                personProjectRepository.Object,
                projectRepository.Object,
                personUserGroupRepository.Object,
                userGroupRepository.Object);

            return (service, personProjectRepository, projectRepository,
                        personUserGroupRepository, userGroupRepository);
        }

        [TestMethod()]
        public async Task GiveAccessToPlantTest()
        {
            const string plantId = "somePlantId";
            const long personId = 2;
            const int amountOfChanges = 3;
            var (service, personProjectRepository,
                projectRepository, personUserGroupRepository,
                userGroupRepository) = Factory();


            //Act
            await service.GiveProjectAccessToPlant(personId, plantId);
            Assert.Fail();
        }

        [TestMethod()]
        public async Task RemoveAccessToPlantTest()
        {
            const string plantId = "somePlantId";
            const long personId = 2;
            const int amountOfChanges = 3;

            //Arrange
            var (service, personProjectRepository, _, _, _) = Factory();

            personProjectRepository.Setup(ppr => ppr.SaveChangesAsync())
                .Returns(Task.FromResult(amountOfChanges));

            //Act
            await service.RemoveAccessToPlant(personId, plantId);

            //assert
            personProjectRepository.Verify(ppr => ppr.SaveChangesAsync(), Times.Once);
            personProjectRepository.Verify(ppr => ppr.VoidPersonProjects(plantId, personId), Times.Once);
        }
    }
}