using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueReceiverService.Services;

namespace QueueReceiverService.Services.Tests
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