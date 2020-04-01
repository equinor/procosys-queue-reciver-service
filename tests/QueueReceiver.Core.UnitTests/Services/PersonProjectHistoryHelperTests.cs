using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class PersonProjectHistoryHelperTests
    {

        [TestMethod]
        public void LogAddAccess()
        {
            //Arrange
            var personProjectHistory = new PersonProjectHistory() { Id = 123 };

            //Act
            PersonProjectHistoryHelper.LogAddAccess(0001, personProjectHistory, 321, "PERSON_CREATED_BY");

            //Assert
            var personProjectHistoryOperation = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperation.OperationType == "INSERT");
            Assert.IsTrue(personProjectHistoryOperation.FieldName == null);
            Assert.IsTrue(personProjectHistoryOperation.NewValue == null);
            Assert.IsTrue(personProjectHistoryOperation.OldValue == null);
            Assert.IsTrue(personProjectHistoryOperation.UpdatedByUser == "PERSON_CREATED_BY");
        }

        [TestMethod]
        public void LogDefaultUserGroup()
        {
            //Arrange
            var personProjectHistory = new PersonProjectHistory() { Id = 234 };

            //Act
            PersonProjectHistoryHelper.LogDefaultUserGroup(0002, personProjectHistory, 432, "PERSON_CREATED_BY");

            //Assert
            var personProjectHistoryOperation = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperation.OperationType == "User role");
            Assert.IsTrue(personProjectHistoryOperation.FieldName == "Read");
            Assert.IsTrue(personProjectHistoryOperation.NewValue == "Y");
            Assert.IsTrue(personProjectHistoryOperation.OldValue == "N");
            Assert.IsTrue(personProjectHistoryOperation.UpdatedByUser == "PERSON_CREATED_BY");
        }

        [TestMethod]
        public void LogVoidProjects()
        {
            //Arrange
            var personProjectHistory = new PersonProjectHistory() { Id = 345 };

            //Act
            PersonProjectHistoryHelper.LogVoidProjects(0003, personProjectHistory, 543, "PERSON_CREATED_BY");

            //Assert
            var personProjectHistoryOperation = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperation.OperationType == "UPDATE");
            Assert.IsTrue(personProjectHistoryOperation.FieldName == "ISVOIDED");
            Assert.IsTrue(personProjectHistoryOperation.NewValue == "Y");
            Assert.IsTrue(personProjectHistoryOperation.OldValue == "N");
            Assert.IsTrue(personProjectHistoryOperation.UpdatedByUser == "PERSON_CREATED_BY");
        }

        [TestMethod]
        public void LogUnvoidProjects()
        {
            //Arrange
            var personProjectHistory = new PersonProjectHistory() { Id = 456 };

            //Act
            PersonProjectHistoryHelper.LogUnvoidProjects(0004, personProjectHistory, 654, "PERSON_CREATED_BY");

            //Assert
            var personProjectHistoryOperation = personProjectHistory.PersonProjectHistoryOperations.FirstOrDefault();

            Assert.IsTrue(personProjectHistory.PersonProjectHistoryOperations.Count == 1);
            Assert.IsTrue(personProjectHistoryOperation.OperationType == "UPDATE");
            Assert.IsTrue(personProjectHistoryOperation.FieldName == "ISVOIDED");
            Assert.IsTrue(personProjectHistoryOperation.OldValue == "Y");
            Assert.IsTrue(personProjectHistoryOperation.NewValue == "N");
            Assert.IsTrue(personProjectHistoryOperation.UpdatedByUser == "PERSON_CREATED_BY");
        }
    }
}
