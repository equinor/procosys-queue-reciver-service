using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using QueueReceiverService.Models;
using QueueReceiverService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReciverServiceTest
{
    [TestClass]
    public class AccessServiceTests
    {
        private readonly Mock<IPersonService> _personService;
        private readonly Mock<IProjectService> _projectService;
        private readonly Mock<IPlantService> _plantService;
        private readonly Mock<ILogger<AccessService>> _logger;
        public AccessServiceTests()
        {
            _personService = new Mock<IPersonService>();
            _projectService = new Mock<IProjectService>();
            _plantService = new Mock<IPlantService>();
            _logger = new Mock<ILogger<AccessService>>();
        }

        public async Task HandleRequest_returns_false_if_plant_doesnt_existAsync()
        {
            //Arrange
            var plantOidThatDoesntExists = "SomePlantThatDoesntExist";
            _plantService.Setup(plantService => plantService.Exists(plantOidThatDoesntExists)).Returns(new ValueTask<bool>(false));

            var service = new AccessService(_personService.Object, _projectService.Object, _plantService.Object, null);
            var accessInfo = new AccessInfo { PlantOid = plantOidThatDoesntExists, Members = null };

            //Act
            var result = await service.HandleRequest(accessInfo);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task HandleRequest_returns_true_when_successfully_removing_access()
        {
            //Arrange
            const string someOid = "someOid";
            const string plantOidThatExists = "SomePlantThatExist";
            _plantService.Setup(plantService => plantService.Exists(plantOidThatExists)).Returns(new ValueTask<bool>(true));

            var service = new AccessService(_personService.Object, _projectService.Object, _plantService.Object, null);

            _personService.Setup(personService => personService.FindOrCreate(someOid, false))
                .Returns(new ValueTask<(Person, bool)>((new Person { Id = 1, Oid = someOid }, true)));

            _projectService.Setup(projectService => projectService.RemoveAccessToPlant(someOid, plantOidThatExists))
                .Returns(new ValueTask<bool>(true));

            var accessService = new AccessService(_personService.Object, _projectService.Object, _plantService.Object, _logger.Object);

            var accessInfo = new AccessInfo
            {
                PlantOid = plantOidThatExists,
                Members = new List<Member>
                {
                    new Member
                    {
                        UserOid = someOid, ShouldRemove = true
                    }
                }
            };

            //Act
            var result = await accessService.HandleRequest(accessInfo);

            //Assert
            Assert.IsTrue(result);
        }
    }
}

