using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.Data;
using QueueReceiver.Infrastructure.Repositories;

namespace QueueReceiver.UnitTests.Infrastructure.Repositories
{
    [TestClass]
    public class PersonRestrictionRoleRepositoryTests
    {
        private const long personId = 2;
        private const string restrictionRole = "NO_RESTRICTION";
        private const string plantId = "Dagobath";


        [TestMethod]
        public async Task AddSync_DoesNothing_IfRoleAlreadyExists()
        {
            //Arrange
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(cxt => cxt.PersonRestrictionRoles
                    .Find(plantId, restrictionRole, personId))
                    .Returns(new PersonRestrictionRole(plantId, restrictionRole, personId));
            mockContext.Setup(cxt => cxt.PersonRestrictionRoles
                    .AddAsync(It.IsAny<PersonRestrictionRole>(), default))
                    .Returns(Task.FromResult(new EntityEntry<PersonRestrictionRole>(new MockInternal())));

            var repository = new PersonRestrictionRoleRepository(mockContext.Object);

            //Act
            await repository.AddIfNotExistAsync(plantId, restrictionRole, personId);

            //Assert
            mockContext.Verify(cxt => cxt.PersonRestrictionRoles
                .Find(plantId, restrictionRole, personId), Times.Once);
            mockContext.Verify(cxt => cxt.PersonRestrictionRoles
                .AddAsync(It.IsAny<PersonRestrictionRole>(), default), Times.Never);
        }

        private class MockInternal : InternalEntityEntry
        {
            private readonly PersonRestrictionRole personRestrictionRole =
                new PersonRestrictionRole(plantId, restrictionRole, personId);

            public MockInternal()
                : base(default, new EntityType("mock", new Model(), default))
            {
            }

            public override object Entity => personRestrictionRole;
        }
    }
}