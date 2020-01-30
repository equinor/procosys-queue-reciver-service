using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using QueueReceiver.Infrastructure.Repositories;
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
            var mockContext = new Mock<QueueReceiverServiceContext>(new DbContextOptions<QueueReceiverServiceContext>());
            mockContext.Setup(cxt => cxt.PersonUserGroups
                    .Find(plantId, personId, userGroupId))
                    .Returns(new PersonUserGroup(personId, userGroupId, plantId, createdById));
            mockContext.Setup(ctx => ctx.PersonUserGroups
                    .AddAsync(It.IsAny<PersonUserGroup>(), default))
                    .Returns(Task.FromResult(new EntityEntry<PersonUserGroup>(new MockInternal())));

            var settings = new DbContextSettings
            {
                PersonProjectCreatedId = createdById
            };

            var repository = new PersonUserGroupRepository(mockContext.Object, settings);

            //Act
            await repository.AddIfNotExistAsync(userGroupId, plantId, personId);

            //Assert
            mockContext.Verify(cxt => cxt.PersonUserGroups
                .Find(plantId, personId, userGroupId), Times.Once);
            mockContext.Verify(context => context.PersonUserGroups
                .AddAsync(It.IsAny<PersonUserGroup>(), default), Times.Never);
        }

        private class MockInternal : InternalEntityEntry
        {
            private readonly PersonUserGroup personUserGroup = new PersonUserGroup(personId, userGroupId, plantId, createdById);

            public MockInternal()
                : base(default, new EntityType("mock", new Model(), default))
            {
            }

            public override object Entity => personUserGroup;
        }
    }
}