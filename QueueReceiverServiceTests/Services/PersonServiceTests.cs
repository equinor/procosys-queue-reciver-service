using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiverService.Models;
using QueueReceiverService.Repositories;
using QueueReceiverService.Services;
using System;
using System.Threading.Tasks;

namespace QueueReciverServiceTests.Services
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
        public async Task FindOrCreate_can_find_by_oid()
        {
            //Arrange
            const int SomeId = 1;
            const string SomeOid = "someOid";
            _personRepository.Setup(personService => personService.FindByUserOid(SomeOid))
                    .Returns(Task.FromResult(new Person { Id = SomeId, Oid = SomeOid }));

            //Act
            var person = await _service.FindOrCreate(SomeOid);

            //Assert
            Assert.AreEqual(SomeId, person.Id);
            Assert.AreEqual(SomeOid, person.Oid);
        }

        [TestMethod]
        public async Task FindOrCreate_can_find_by_username()
        {
            //Arrange
            const int SomeId = 1;
            const string someUsername = "someUsername";
            const string SomeOid = "someOid";

            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
                .Returns(Task.FromResult(new AdPerson(SomeOid, someUsername, "anyEmail")));
            _personRepository.Setup(personService => personService.FindByUsername(someUsername))
                .Returns(Task.FromResult(new Person { Id = SomeId, UserName = someUsername }));
            _personRepository.Setup(personService => personService.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            //Act
            var person = await _service.FindOrCreate(SomeOid);

            //Assert
            Assert.AreEqual(SomeId, person.Id);
        }

        [TestMethod]
        public async Task FindOrCreate_can_find_by_email()
        {
            //Arrange
            const int SomeId = 1;
            const string someEmail = "someEmail";
            const string SomeOid = "someOid";

            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
                .Returns(Task.FromResult(new AdPerson(SomeOid, "anyUserName", someEmail)));
            _personRepository.Setup(personService => personService.FindByUserEmail(someEmail))
                .Returns(Task.FromResult(new Person { Id = SomeId, Email = someEmail }));
            _personRepository.Setup(personService => personService.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            //Act
            var person= await _service.FindOrCreate(SomeOid);

            //Assert
            Assert.AreEqual(SomeId, person.Id);
        }

        [TestMethod]
        public async Task FindByOid_returns_null_if_no_person_in_db()
        {
            //Arrange
            const string SomeOid = "someOid";
            _graphService.Setup(graphService => graphService.GetPersonByOid(SomeOid))
                .Returns(Task.FromResult(new AdPerson(SomeOid, "anyUserName", "anyEmail")));

            //Act
            var person  = await _service.FindByOid(SomeOid);

            Assert.IsNull(person);
        }
    }
}
