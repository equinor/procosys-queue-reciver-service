using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using QueueReceiver.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.UnitTests.Repositories
{
    [TestClass]
    public class PersonUserGroupRepositoryTests
    {
        private const long personId = 1;
        private const long userGroupId = 1;
        private const string plantId = "Dagobath";
        private const long createdById = 1212;

        [TestMethod]
        public async Task AddAsync_DoesNothing_IfGroupAlreadyExists()
        {
            //Arrange
            var personUserGroups = new List<PersonUserGroup>
            {
                new PersonUserGroup(personId,userGroupId,plantId,createdById)
            };

            var mockSet = personUserGroups.AsQueryable().BuildMockDbSet();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(cxt => cxt.PersonUserGroups).Returns(mockSet.Object);
            mockContext.Setup(ctx => ctx.PersonUserGroups.AddAsync(It.IsAny<PersonUserGroup>(), default))
                .Returns(Task.FromResult(new EntityEntry<PersonUserGroup>(new MockInternal())));

            var settings = new DbContextSettings
            {
                PersonProjectCreatedId = createdById
            };

            var repository = new PersonUserGroupRepository(mockContext.Object, settings);

            //Act
            await repository.AddAsync(userGroupId, plantId, personId);

            //Assert
            mockContext.Verify(context => context.PersonUserGroups.AddAsync(It.IsAny<PersonUserGroup>(), default), Times.Never);
        }


        private class MockInternal : InternalEntityEntry
        {
            private readonly PersonUserGroup personUserGroup = new PersonUserGroup(personId, userGroupId, plantId, createdById);

            public MockInternal()
                : base(default, new EntityType("mock", new Model(),default))
            {
            }

            public override object Entity => personUserGroup;
        }
    }
}