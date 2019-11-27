using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueReceiver.Core.Interfaces;

namespace QueueReceiver.UnitTests.Core.Services
{
    [TestClass()]
    public class PlantServiceTests
    {
        private readonly IPlantService _plantService;
        public PlantServiceTests(IPlantService plantService)
        {
            _plantService = plantService;
        }


        [TestMethod()]
        public void ExistsTest()
        {
            //Arrange
            const string existingPlantOid = "someOid";


            _plantService.Exists(existingPlantOid);
            Assert.Fail();
        }
    }
}