using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using QueueReceiverService.Models;
using QueueReceiverService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace QueueReciverServiceTest.Services
{
    [TestClass]
    public class AccessServiceTests
    {
        private readonly Mock<IPersonService> _personService;
        private readonly Mock<IProjectService> _projectService;
        private readonly Mock<IPlantService> _plantService;
        private readonly Mock<ILogger<AccessService>> _logger;
        private readonly IAccessService _service;
        public AccessServiceTests()
        {
            _personService = new Mock<IPersonService>();
            _projectService = new Mock<IProjectService>();
            _plantService = new Mock<IPlantService>();
            _logger = new Mock<ILogger<AccessService>>();
            _service = new AccessService(
                _personService.Object, 
                _projectService.Object,
                _plantService.Object, 
                _logger.Object);
        }

        [TestMethod]
        public async Task HandleRequest_returns_early_if_plant_doesnt_existAsync()
        {
            //Arrange
            const string plantOidThatDoesntExists = "SomePlantThatDoesNotExist";
            _plantService.Setup(plantService => plantService.Exists(plantOidThatDoesntExists))
                .Returns(Task.FromResult(false));
            var accessInfo = new AccessInfo { PlantOid = plantOidThatDoesntExists, Members = null };

            //Act
             await _service.HandleRequest(accessInfo);

            //Assert
            _plantService.Verify(_ => _.GetPlantId(plantOidThatDoesntExists), Times.Once);
            _personService.Verify(_ => _.FindOrCreate(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public async Task HandleRequest_successfully_removes_access()
        {
            //Arrange
            const bool ShouldNotCreate = true;
            const string someOid = "someOid";
            const long somePersonId = 12;
            const string somePlantId = "testPlant";
            const string plantOidThatExists = "SomePlantThatExist";
            _plantService.Setup(plantService => plantService.GetPlantId(plantOidThatExists))
                .Returns(Task.FromResult(somePlantId));
            _personService.Setup(personService => personService.FindOrCreate(someOid, ShouldNotCreate))
                .Returns(Task.FromResult(new Person { Id = 1, Oid = someOid }));
            _projectService.Setup(projectService => projectService.RemoveAccessToPlant(somePersonId, plantOidThatExists))
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
             await _service.HandleRequest(accessInfo);

            //Assert
            _personService.Verify(_=> _.FindOrCreate(someOid, false), Times.Once);
            _projectService.Verify(_=> _.RemoveAccessToPlant(somePersonId,It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task HandleRequest_returns_early_when_removing_access_for_user_not_in_db()
        {
            //Arrange
            _plantService.Setup(plantService => plantService.Exists(It.IsAny<string>()))
                .Returns(Task.FromResult(true));
        
            var accessInfo = new AccessInfo
            {
                PlantOid = "anyPlantOid",
                Members = new List<Member>
                {
                    new Member{ UserOid = "anyOid", ShouldRemove = true }
                }
            };

            //Act
            await _service.HandleRequest(accessInfo);

            //Assert
            _projectService.Verify(_ => _.RemoveAccessToPlant(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task HandleRequest_returns_false_if_any_member_fails()
        {
            //Arrange
            var person1 = new Person { Id = 1, Oid = "oid1" };

            _plantService.Setup(plantService => plantService.GetPlantId(It.IsAny<string>()))
                .Returns(Task.FromResult("somePlantId"));
            _personService.Setup(personService => personService.FindOrCreate("oid1", false))
                .Returns(Task.FromResult(person1));
            _personService.Setup(personService => personService.FindOrCreate("oid2", false))
                .Throws(new Exception("Any Exception"));
            _projectService.Setup(projectService => projectService.GiveProjectAccessToPlant(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

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
            await _service.HandleRequest(accessInfo);

            //Assert
            _projectService.Verify(_ =>
                _.GiveProjectAccessToPlant(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
            _projectService.Verify(_ =>
                _.RemoveAccessToPlant(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
    }
}

