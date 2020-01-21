using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class AccessServiceTests
    {
        private readonly Mock<IPersonService> _personService;
        private readonly Mock<IPersonProjectService> _personProjectService;
        private readonly Mock<IPlantService> _plantService;
        private readonly Mock<ILogger<AccessService>> _logger;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly IAccessService _service;
        public AccessServiceTests()
        {
            _personService = new Mock<IPersonService>();
            _personProjectService = new Mock<IPersonProjectService>();
            _plantService = new Mock<IPlantService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _logger = new Mock<ILogger<AccessService>>();
            _service = new AccessService(
                _personService.Object,
                _personProjectService.Object,
                _plantService.Object,
                _logger.Object,
                _unitOfWork.Object);
        }

        [TestMethod]
        public async Task HandleRequest_returns_early_if_plant_doesnt_existAsync()
        {
            //Arrange
            const string plantOidThatDoesntExists = "SomePlantThatDoesNotExist";
            _plantService.Setup(plantService => plantService.GetPlantId(plantOidThatDoesntExists))
                .Returns(Task.FromResult<string?>(null));
            var accessInfo = new AccessInfo(plantOidThatDoesntExists, new List<Member>());

            //Act
            await _service.HandleRequest(accessInfo);

            //Assert
            _plantService.Verify(_ => _.GetPlantId(plantOidThatDoesntExists), Times.Once);
            _personService.Verify(_ => _.CreateIfNotExist(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task HandleRequest_successfully_removes_access()
        {
            //Arrange
            const string someOid = "someOid";
            const long somePersonId = 12;
            const string somePlantId = "testPlant";
            const string plantOidThatExists = "SomePlantThatExist";
            _plantService.Setup(plantService => plantService.GetPlantId(plantOidThatExists))
                .Returns(Task.FromResult(somePlantId)!);
            _personService.Setup(personService => personService.UpdateWithOidIfNotFound(someOid))
                .Returns(Task.FromResult(new Person("", "") { Id = somePersonId, Oid = someOid })!);
            //_personProjectService.Setup(personProjectService => personProjectService.RemoveAccessToPlant(somePersonId, plantOidThatExists))
              //  .Returns(Task.FromResult(true));

            var accessInfo = new AccessInfo(plantOidThatExists, new List<Member>
                {
                    new Member(someOid, true)
                });

            //Act
            await _service.HandleRequest(accessInfo);

            //Assert
            _personService.Verify(_ => _.UpdateWithOidIfNotFound(someOid), Times.Once);
            _personProjectService.Verify(_ => _.RemoveAccessToPlant(somePersonId, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task HandleRequest_returns_early_when_removing_access_for_user_not_in_db()
        {
            //Arrange
            _plantService.Setup(plantService => plantService.GetPlantId(It.IsAny<string>()))
                .Returns(Task.FromResult<string?>("anyPlantOid"));

            var accessInfo = new AccessInfo(
                "anyPlantOid",
                new List<Member> { new Member("anyOid", true) }
                );

            //Act
            await _service.HandleRequest(accessInfo);

            //Assert
            _personProjectService.Verify(_ => _.RemoveAccessToPlant(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
    }
}

