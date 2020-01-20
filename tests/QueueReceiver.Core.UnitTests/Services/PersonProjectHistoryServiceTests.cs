using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class PersonProjectHistoryServiceTests
    {
        private static PersonProjectHistoryService 

            Factory()
        {
            var service = new PersonProjectHistoryService();
            return service;
        }

        [TestMethod]
        public void LogAddAccess()
        {
            //Arrange
            var service = Factory();
            var personProjectHistory = new PersonProjectHistory() { Id = 123 };

            //Act
            service.LogAddAccess(0001, personProjectHistory, 321);

            //Assert
            var personProjectHistoryOperation = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperation.UpdatedByUser == PersonProjectHistoryConstants.UpdatedBy);
            Assert.IsTrue(personProjectHistoryOperation.OperationType == "INSERT");
            Assert.IsTrue(personProjectHistoryOperation.FieldName == null);
            Assert.IsTrue(personProjectHistoryOperation.NewValue == null);
            Assert.IsTrue(personProjectHistoryOperation.OldValue == null);
        }

        [TestMethod]
        public void LogDefaultUserGroup()
        {
            //Arrange
            var service = Factory();
            var personProjectHistory = new PersonProjectHistory() { Id = 234 };

            //Act
            service.LogDefaultUserGroup(0002, personProjectHistory, 432);

            //Assert
            var personProjectHistoryOperation = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperation.UpdatedByUser == PersonProjectHistoryConstants.UpdatedBy);
            Assert.IsTrue(personProjectHistoryOperation.OperationType == "User role");
            Assert.IsTrue(personProjectHistoryOperation.FieldName == "Read");
            Assert.IsTrue(personProjectHistoryOperation.NewValue == "Y");
            Assert.IsTrue(personProjectHistoryOperation.OldValue == "N");
        }

        [TestMethod]
        public void LogVoidProjects()
        {
            //Arrange
            var service = Factory();
            var personProjectHistory = new PersonProjectHistory() { Id = 345 };

            //Act
            service.LogVoidProjects(0003, personProjectHistory, 543);

            //Assert
            var personProjectHistoryOperation = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperation.UpdatedByUser == PersonProjectHistoryConstants.UpdatedBy);
            Assert.IsTrue(personProjectHistoryOperation.OperationType == "UPDATE");
            Assert.IsTrue(personProjectHistoryOperation.FieldName == "ISVOIDED");
            Assert.IsTrue(personProjectHistoryOperation.NewValue == "Y");
            Assert.IsTrue(personProjectHistoryOperation.OldValue == "N");
        }

        [TestMethod]
        public void LogUnvoidProjects()
        {
            //Arrange
            var service = Factory();
            var personProjectHistory = new PersonProjectHistory() { Id = 456 };

            //Act
            service.LogUnvoidProjects(0004, personProjectHistory, 654);

            //Assert
            var personProjectHistoryOperations = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperations.UpdatedByUser == PersonProjectHistoryConstants.UpdatedBy);
            Assert.IsTrue(personProjectHistoryOperations.OperationType == "UPDATE");
            Assert.IsTrue(personProjectHistoryOperations.FieldName == "ISVOIDED");
            Assert.IsTrue(personProjectHistoryOperations.OldValue == "Y");
            Assert.IsTrue(personProjectHistoryOperations.NewValue == "N");
        }
    }
}
