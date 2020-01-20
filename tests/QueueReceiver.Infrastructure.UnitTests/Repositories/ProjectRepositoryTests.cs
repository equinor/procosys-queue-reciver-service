using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using QueueReceiver.Core.Models;
using QueueReceiver.Infrastructure.EntityConfiguration;
using QueueReceiver.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueReceiver.Infrastructure.UnitTests.Repositories
{
    [TestClass]
    public class ProjectRepositoryTests
    {
        [TestMethod]
        public async Task GetParentProjectsByPlantTest()
        {
            //Arrange
            const string plantA = "plantA";
            const string plantB = "plantB";
            const int projectId = 5;

            var projects = new List<Project>
            {
                new Project
                {
                    ParentProjectId = 1,
                    PlantId = plantA,
                    IsVoided = false
                },
                new Project
                {
                    ParentProjectId = 1,
                    PlantId = plantB,
                    IsVoided = false
                },
                new Project
                {
                    ParentProjectId = 1,
                    PlantId = plantA,
                    IsVoided = true
                },
                new Project
                {
                    ProjectId = projectId,
                    ParentProjectId = null,
                    PlantId = plantA,
                    IsVoided = false
                }
            };

            var mockSet = projects.AsQueryable().BuildMockDbSet();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(cxt => cxt.Projects).Returns(mockSet.Object);

            var repository = new ProjectRepository(mockContext.Object);

            //Act
            var result = await repository.GetParentProjectsByPlant(plantA);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(projectId, result[0].ProjectId);
        }
    }
}