using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using QueueReceiver.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.UnitTests.Infrastructure.Repositories
{
    [TestClass]
    public class PersonUserGroupRepositoryTests
    {
        [TestMethod]
        public async Task AddAsync_DoesNothing_IfGroupAlreadyExists()
        {
            //Arrange
            const long peronId = 1;
            const long userGroupId = 1;
            const string plantId = "Dagobath";
            const long createdById = 1212;

            var personUserGroups = new List<PersonUserGroup>
            {
                new PersonUserGroup(peronId,userGroupId,plantId,createdById)
            };

            var mockSet = personUserGroups.AsQueryable().BuildMockDbSet();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(cxt => cxt.PersonUserGroups).Returns(mockSet.Object);
            mockContext.Setup(ctx => ctx.PersonUserGroups.AddAsync(It.IsAny<PersonUserGroup>(), default))
                .Returns(Task.FromResult(new EntityEntry<PersonUserGroup>(It.IsAny<InternalEntityEntry>())));

            var settings = new DbContextSettings {
                PersonProjectCreatedId = createdById
            };

            var repository = new PersonUserGroupRepository(mockContext.Object, settings);

            //Act
            await repository.AddAsync(userGroupId, plantId, peronId);

            //Assert
            mockContext.Verify(context => context.AddAsync(It.IsAny<PersonUserGroup>(),default), Times.Never);
        }
    }
}