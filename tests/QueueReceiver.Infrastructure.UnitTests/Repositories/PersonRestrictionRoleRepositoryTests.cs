using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueReceiver.Core.Interfaces;

namespace QueueReceiver.UnitTests.Infrastructure.Repositories
{
    [TestClass]
    public class PersonRestrictionRoleRepositoryTests
    {
        public PersonRestrictionRoleRepositoryTests(
            IPersonRestrictionRoleRepository personRestrictionRoleRepository)
        {
            _personRestrictionRoleRepository = personRestrictionRoleRepository;
        }

        private IPersonRestrictionRoleRepository _personRestrictionRoleRepository;

        [TestMethod]
        public async Task AddIfNotExistTest()
        {
            // Testing AddIfNotExistAsync(string plantId, string restrictionRole, long personId)
            
            
            //Arrange
            const string plantId = "PCS$HEIMDAL";
            const string restrictionRole = "NO_RESTRICTION";
            const long personId = 1;

            //Act
            var x = _personRestrictionRoleRepository.AddIfNotExistAsync(plantId, restrictionRole, personId);

            //Assert
            
        }
    }
}
