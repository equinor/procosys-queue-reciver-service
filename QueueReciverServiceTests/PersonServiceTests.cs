using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiverService.Models;
using QueueReceiverService.Repositories;
using QueueReceiverService.Services;
using System.Threading.Tasks;

namespace QueueReciverServiceTests
{
    [TestClass]
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _personRepository;
        private readonly Mock<IGraphService> _graphService;

        public PersonServiceTests()
        {
            _personRepository = new Mock<IPersonRepository>();
            _graphService = new Mock<IGraphService>();
        }

        [TestMethod]
        public async Task FindOrCreate_can_find_by_oid()
        {
            //Arrange
            const int SomeId = 1;
            const string SomeOid = "someOid";
            _personRepository.Setup(personService => personService.FindByUserOid(SomeOid))
                    .Returns(Task.FromResult(new Person { Id = SomeId, Oid = SomeOid }));
            var service = new PersonService(_personRepository.Object, _graphService.Object);

            //Act
            var (person, success) = await service.FindOrCreate(SomeOid);

            //Assert
            Assert.AreEqual(SomeId, person.Id);
            Assert.AreEqual(SomeOid, person.Oid);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task FindOrCreate_can_find_by_username()
        {
            //Arrange
            const int SomeId = 1;
            const string someUsername = "someUsername";
            const string SomeOid = "someOid";

            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
                .Returns(Task.FromResult(
                                new AdPerson
                                {
                                    Oid = SomeOid,
                                    Username = someUsername,
                                    Email = "anyEmail"
                                }));

            _personRepository.Setup(personService => personService.FindByUsername(someUsername))
                    .Returns(Task.FromResult(new Person { Id = SomeId, UserName = someUsername }));
            _personRepository.Setup(personService => personService.SaveChangesAsync()).Returns(Task.FromResult(true));

            var service = new PersonService(_personRepository.Object, _graphService.Object);

            //Act
            var (person, success) = await service.FindOrCreate(SomeOid);

            //Assert
            Assert.AreEqual(SomeId, person.Id);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public async Task FindOrCreate_can_find_by_email()
        {
            //Arrange
            const int SomeId = 1;
            const string someEmail = "someEmail";
            const string SomeOid = "someOid";

            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
                .Returns(Task.FromResult(
                                new AdPerson
                                {
                                    Oid = SomeOid,
                                    Username = "anyUsername",
                                    Email = someEmail
                                }));

            _personRepository.Setup(personService => personService.FindByUserEmail(someEmail))
                    .Returns(Task.FromResult(new Person { Id = SomeId, Email = someEmail }));
            _personRepository.Setup(personService => personService.SaveChangesAsync()).Returns(Task.FromResult(true));

            var service = new PersonService(_personRepository.Object, _graphService.Object);

            //Act
            var (person, success) = await service.FindOrCreate(SomeOid);

            //Assert
            Assert.AreEqual(SomeId, person.Id);
            Assert.IsTrue(success);
        }


        [TestMethod]
        public async Task FindOrCreate_returns_failure_if_save_fails()
        {
            //Arrange
            const string SomeOid = "someOid";

            _personRepository.Setup(personService => personService.SaveChangesAsync()).Returns(Task.FromResult(false));

            var service = new PersonService(_personRepository.Object, _graphService.Object);
            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
             .Returns(Task.FromResult(
                             new AdPerson
                             {
                                 Oid = SomeOid,
                                 Username = "anyUserName",
                                 Email = "anyEmail"
                             }));


            //Act
            var (person, success) = await service.FindOrCreate(SomeOid);

            //Assert
            Assert.IsFalse(success);
        }


        [TestMethod]
        public async Task FindOrCreate_returns_null_and_success_if_shouldNotCreate()
        {
            //Arrange
            const string SomeOid = "someOid";
            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
            .Returns(Task.FromResult(
                            new AdPerson
                            {
                                Oid = SomeOid,
                                Username = "anyUserName",
                                Email = "anyEmail"
                            }));

            var service = new PersonService(_personRepository.Object, _graphService.Object);

            //Act
            var (person, success) = await service.FindOrCreate(SomeOid, shouldNotCreate: true);

            Assert.IsNull(person);
            Assert.IsTrue(success);
        }
    }
}
