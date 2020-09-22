using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using System.Threading.Tasks;
using QueueReceiver.Core.Constants;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _personRepository;
        private readonly Mock<IGraphService> _graphService;
        private readonly IPersonService _service;
        private readonly Mock<IProjectRepository> _projectRepository;
        private readonly Mock<IPersonProjectRepository> _personProjectRepository;
        private readonly Mock<ILogger<PersonService>> _personServiceLogger;
        private readonly PersonCreatedByCache _personCreatedByCache;

        public PersonServiceTests()
        {
            _personRepository = new Mock<IPersonRepository>();
            _graphService = new Mock<IGraphService>();
            _projectRepository = new Mock<IProjectRepository>();
            _personProjectRepository = new Mock<IPersonProjectRepository>();
            _personServiceLogger = new Mock<ILogger<PersonService>>();
            _personCreatedByCache = new PersonCreatedByCache(111);
            _service = new PersonService(
                _personRepository.Object,
                _graphService.Object,
                _projectRepository.Object,
                _personCreatedByCache,
                _personProjectRepository.Object,
                _personServiceLogger.Object);
        }

        [TestMethod]
        public async Task FindByOid_Works()
        {
            //Arrange
            const int SomeId = 1;
            const string SomeOid = "someOid";
            _personRepository.Setup(personService => personService.FindByUserOidAsync(SomeOid))
                    .Returns(Task.FromResult<Person?>(new Person("", "") { Id = SomeId, Oid = SomeOid }));

            //Act
            var person = await _service.FindPersonByOidAsync(SomeOid);

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
            const string SomePlantId = "somePlantId";

            _graphService.Setup(graphService => graphService.GetAdPersonByOidAsync(SomeOid))
                .Returns(Task.FromResult<AdPerson?>(new AdPerson(SomeOid, "anything", "anyEmail")
                {
                    MobileNumber = MobileNo,
                    GivenName = GivenName,
                    Surname = Surname
                }));
            _personRepository.Setup(repo => repo.FindByMobileNumberAndNameAsync(MobileNo, GivenName, Surname))
                .Returns(Task.FromResult<Person?>(new Person("tull", "tøys") { Id = SomeId }));
            _personRepository.Setup(repo => repo.FindByUserOidAsync(SomeOid))
                .Returns(Task.FromResult<Person?>(new Person("tull", "tøys") { Id = SomeId }));

            //Act
            await _service.CreateIfNotExist(SomeOid, SomePlantId);

            //Assert
            var person = await _service.FindPersonByOidAsync(SomeOid);
            Assert.AreEqual(SomeId, person?.Id);
        }

        [TestMethod]
        public async Task FindByOid_ReturnsNull_IfPersonNotInDb()
        {
            //Arrange
            const string SomeOid = "someOid";
            _graphService.Setup(graphService => graphService.GetAdPersonByOidAsync(SomeOid))
                .Returns(Task.FromResult<AdPerson?>(new AdPerson(SomeOid, "anyUserName", "anyEmail")));

            //Act
            var person = await _service.UpdateWithOidIfNotFound(SomeOid);

            //Assert
            Assert.IsNull(person);
        }

        [TestMethod]
        public void GetAdPersonNameFromGivenAndSurname()
        {
            // Arrange
            const string testGivenName = "TestGivenName";
            const string testSurname = "TestSurname";

            var adPerson = new AdPerson("oid", "username", "email")
            {
                GivenName = testGivenName,
                Surname = testSurname
            };

            // Act
            var (firstName, lastName) = _service.GetAdPersonFirstAndLastName(adPerson);

            // Assert
            Assert.AreEqual(testGivenName, firstName);
            Assert.AreEqual(testSurname, lastName);
        }

        [TestMethod]
        public void GetAdPersonNameFromDisplayName()
        {
            // Arrange
            const string testDisplayName = "Jul E Nissen";

            var adPerson = new AdPerson("oid", "username", "email")
            {
                DisplayName = testDisplayName
            };

            // Act
            var (firstName, lastName) = _service.GetAdPersonFirstAndLastName(adPerson);

            // Assert
            Assert.AreEqual("Jul E", firstName);
            Assert.AreEqual("Nissen", lastName);
        }

        [TestMethod]
        public void GetAdPersonNameFromDisplayNameWithCommaError()
        {
            // Arrange
            const string testDisplayName = "Lastname, Firstname Middle";

            var adPerson = new AdPerson("oid", "username", "email")
            {
                DisplayName = testDisplayName
            };

            // Act
            var (firstName, lastName) = _service.GetAdPersonFirstAndLastName(adPerson);

            // Assert
            Assert.AreEqual("Firstname Middle", firstName);
            Assert.AreEqual("Lastname", lastName);
        }

        [TestMethod]
        public void GetEmailAddressDomain()
        {
            // Arrange
            const string testUserNameAsEmail = "foo@bar.com";
            const string testUserNameWithoutEmail = "foo";

            // Act
            var emailDomainFromUsername = _service.GetEmailAddressDomain(testUserNameAsEmail);
            var emailDomainFromDefault = _service.GetEmailAddressDomain(testUserNameWithoutEmail);

            // Assert
            Assert.AreEqual("BAR.COM", emailDomainFromUsername);
            Assert.AreEqual(ReconcileConstants.DefaultEmailDomain, emailDomainFromDefault);
        }
    }
}
