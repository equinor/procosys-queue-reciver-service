using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using System.Threading.Tasks;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _personRepository;
        private readonly Mock<IGraphService> _graphService;
        private readonly IPersonService _service;

        public PersonServiceTests()
        {
            _personRepository = new Mock<IPersonRepository>();
            _graphService = new Mock<IGraphService>();
            _service = new PersonService(_personRepository.Object, _graphService.Object);
        }

        [TestMethod]
        public async Task FindByOid_Works()
        {
            //Arrange
            const int SomeId = 1;
            const string SomeOid = "someOid";
            _personRepository.Setup(personService => personService.FindByUserOid(SomeOid))
                    .Returns(Task.FromResult<Person?>(new Person("", "") { Id = SomeId, Oid = SomeOid }));

            //Act
            var person = await _service.FindByOid(SomeOid);

            //Assert
            Assert.IsNotNull(person);
            Assert.AreEqual(SomeId, person!.Id);
            Assert.AreEqual(SomeOid, person.Oid);
        }

        [TestMethod]
        public async Task FindOrCreate_CanFindByNameAndMobileNumber()
        {
            //Arrange
            const int SomeId = 1;
            const string GivenName = "Herman August";
            const string Surname = "Kronglevåg";
            const string SomeOid = "someOid";
            const string MobileNo = "762982109";

            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
                .Returns(Task.FromResult<AdPerson?>(new AdPerson(SomeOid, "anything", "anyEmail")
                {
                    MobileNumber = MobileNo,
                    GivenName = GivenName,
                    Surname = Surname
                }));
            _personRepository.Setup(repo => repo.FindByMobileNumberAndName(MobileNo, GivenName, Surname))
                .Returns(Task.FromResult<Person?>(new Person("tull", "tøys") { Id = SomeId }));

            //Act
            var person = await _service.CreateIfNotExist(SomeOid);

            //Assert
            Assert.AreEqual(SomeId, person.Id);
        }

        [TestMethod]
        public async Task FindByOid_ReturnsNull_IfPersonNotInDb()
        {
            //Arrange
            const string SomeOid = "someOid";
            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
                .Returns(Task.FromResult<AdPerson?>(new AdPerson(SomeOid, "anyUserName", "anyEmail")));

            //Act
            var person = await _service.UpdateWithOidIfNotFound(SomeOid);

            //Assert
            Assert.IsNull(person);
        }
    }
}
