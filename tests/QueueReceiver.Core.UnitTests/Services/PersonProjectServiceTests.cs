﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.UnitTests.Services
{
    [TestClass]
    public class PersonProjectServiceTests
    {
        private static (ProjectService,
            Mock<IPersonProjectRepository>,
            Mock<IProjectRepository>,
            Mock<IPersonUserGroupRepository>,
            Mock<IUserGroupRepository>,
            Mock<IPersonRestrictionRoleRepository>,
            Mock<IRestrictionRoleRepository>)

            Factory()
        {
            var personProjectRepository = new Mock<IPersonProjectRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var personUserGroupRepository = new Mock<IPersonUserGroupRepository>();
            var userGroupRepository = new Mock<IUserGroupRepository>();
            var personRestrictionRoleRepository = new Mock<IPersonRestrictionRoleRepository>();
            var restrictionRoleRepository = new Mock<IRestrictionRoleRepository>();

            var service = new ProjectService(
                personProjectRepository.Object,
                projectRepository.Object,
                personUserGroupRepository.Object,
                userGroupRepository.Object,
                personRestrictionRoleRepository.Object,
                restrictionRoleRepository.Object);

            return (service, personProjectRepository, projectRepository, personUserGroupRepository, userGroupRepository,
                personRestrictionRoleRepository, restrictionRoleRepository);
        }

        [TestMethod]
        public async Task GiveAccessToPlant_CallsCorrectMethods()
        {
            //Arrange
            const string plantId = "somePlantId";
            const long personId = 2;
            const long projectId = 15;
            var (service, personProjectRepository, projectRepository, _, _, _, _) = Factory();
            projectRepository.Setup(pr => pr.GetParentProjectsByPlant(plantId))
                .Returns(Task.FromResult(new List<Project> { new Project { PlantId = plantId, ProjectId = projectId } }));

            //Act
            await service.GiveProjectAccessToPlant(personId, plantId);

            //Assert
            personProjectRepository.Verify(ppr => ppr.AddAsync(projectId, personId), Times.Once);
        }

        [TestMethod]
        public void RemoveAccessToPlant_CallsCorrectMethods()
        {
            //Arrange
            const string plantId = "somePlantId";
            const long personId = 2;

            //Arrange
            var (service, personProjectRepository, _, _, _, _, _) = Factory();

            //Act
            service.RemoveAccessToPlant(personId, plantId);

            //Assert
            personProjectRepository.Verify(ppr => ppr.VoidPersonProjects(plantId, personId), Times.Once);
        }
    }
}