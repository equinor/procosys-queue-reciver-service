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
        private readonly IAccessService service;
        public AccessServiceTests()
        {
            _personService = new Mock<IPersonService>();
            _projectService = new Mock<IProjectService>();
            _plantService = new Mock<IPlantService>();
            _logger = new Mock<ILogger<AccessService>>();
            service = new AccessService(
                _personService.Object, 
                _projectService.Object,
                _plantService.Object, 
                _logger.Object);
        }

        [TestMethod]
        public async Task HandleRequest_returns_false_if_plant_doesnt_existAsync()
        {
            //Arrange
            var plantOidThatDoesntExists = "SomePlantThatDoesNotExist";
            _plantService.Setup(plantService => plantService.Exists(plantOidThatDoesntExists)).Returns(Task.FromResult(false));

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
            const bool ShouldNotCreate = true;
            const string someOid = "someOid";
            const string plantOidThatExists = "SomePlantThatExist";
            _plantService.Setup(plantService => plantService.Exists(plantOidThatExists)).Returns(Task.FromResult(true));
            _personService.Setup(personService => personService.FindOrCreate(someOid, ShouldNotCreate))
                .Returns(new ValueTask<(Person, bool)>((new Person { Id = 1, Oid = someOid }, true)));
            _projectService.Setup(projectService => projectService.RemoveAccessToPlant(someOid, plantOidThatExists))
                .Returns(Task.FromResult(true));

            var accessInfo = new AccessInfo
            {
                PlantOid = plantOidThatExists,
                Members = new List<Member>
                {
                    new Member{ UserOid = someOid, ShouldRemove = true}
                }
            };

            //Act
            var result = await service.HandleRequest(accessInfo);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task HandleRequest_returns_true_when_removing_access_for_user_not_in_db()
        {
            //Arrange
            _plantService.Setup(plantService => plantService.Exists(It.IsAny<string>())).Returns(Task.FromResult(true));

            _personService.Setup(personService => personService.FindOrCreate(It.IsAny<string>(), true))
               .Returns(new ValueTask<(Person, bool)>((null, true)));

            var accessInfo = new AccessInfo
            {
                PlantOid = "anyPlantOid",
                Members = new List<Member>
                {
                    new Member{ UserOid = "anyOid", ShouldRemove = true }
                }
            };

            //Act
            var result = await service.HandleRequest(accessInfo);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task HandleRequest_returns_false_if_any_member_fails()
        {
            //Arrange
            var person1 = new Person { Id = 1, Oid = "oid1" };
            var person2 = new Person { Id = 1, Oid = "oid2" };

            _plantService.Setup(plantService => plantService.Exists(It.IsAny<string>())).Returns(Task.FromResult(true));
            _personService.Setup(personService => personService.FindOrCreate("oid1", false))
             .Returns(new ValueTask<(Person, bool)>((person1, true)));
            _personService.Setup(personService => personService.FindOrCreate("oid2", false))
            .Returns(new ValueTask<(Person, bool)>((person2, false)));
            _projectService.Setup(projectService => projectService.GiveAccessToPlant(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var accessInfo = new AccessInfo
            {
                PlantOid = "anyPlantOid",
                Members = new List<Member>
                {
                    new Member { UserOid = "oid1", ShouldRemove = false },
                    new Member { UserOid = "oid2", ShouldRemove = false }
                }
            };

            //Act
            var result = await service.HandleRequest(accessInfo);

            //Assert
            Assert.IsFalse(result);
        }
    }
}

