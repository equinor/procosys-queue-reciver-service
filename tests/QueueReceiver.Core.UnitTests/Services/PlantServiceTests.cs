﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Services;
using System.Threading.Tasks;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class PlantServiceTests
    {
        private readonly Mock<IPlantRepository> _mockRepository;
        private readonly IPlantService _plantService;

        public PlantServiceTests()
        {
            _mockRepository = new Mock<IPlantRepository>();
            _plantService = new PlantService(_mockRepository.Object);
        }

        [TestMethod]
        public async Task GetPlantIdTest()
        {
            //Arrange
            const string existingPlantOid = "someOid";
            const string existingPlantId = "somePlantId";

            _mockRepository.Setup(r => r.GetPlantIdByOidAsync(existingPlantOid))
                .Returns(Task.FromResult<string?>(existingPlantId));

            //Act
            var result = await _plantService.GetPlantIdAsync(existingPlantOid);

            //Assert
            Assert.AreEqual(existingPlantId, result);
        }
    }
}