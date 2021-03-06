﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private static (PersonProjectService,
            Mock<IPersonProjectRepository>,
            Mock<IProjectRepository>,
            Mock<IPersonUserGroupRepository>,
            Mock<IUserGroupRepository>,
            Mock<IPersonRestrictionRoleRepository>,
            Mock<IRestrictionRoleRepository>,
            Mock<IPersonProjectHistoryRepository>,
            Mock<IPersonService>,
            PersonCreatedByCache
            )

            Factory()
        {
            var personProjectRepository = new Mock<IPersonProjectRepository>();
            var projectRepository = new Mock<IProjectRepository>();
            var personUserGroupRepository = new Mock<IPersonUserGroupRepository>();
            var userGroupRepository = new Mock<IUserGroupRepository>();
            var personRestrictionRoleRepository = new Mock<IPersonRestrictionRoleRepository>();
            var restrictionRoleRepository = new Mock<IRestrictionRoleRepository>();
            var personProjectHistoryRepository = new Mock<IPersonProjectHistoryRepository>();
            var privilegeService = new Mock<IPrivilegeService>();
            var personService = new Mock<IPersonService>();
            var logger = new Mock<ILogger<PersonProjectService>>();
            var personCreatedByCache = new PersonCreatedByCache(111);

            var service = new PersonProjectService(
                personProjectRepository.Object,
                projectRepository.Object,
                privilegeService.Object,
                personProjectHistoryRepository.Object,
                personService.Object,
                logger.Object,
                personCreatedByCache);

            return (service, personProjectRepository, projectRepository, personUserGroupRepository, userGroupRepository,
                personRestrictionRoleRepository, restrictionRoleRepository, personProjectHistoryRepository,
                personService, personCreatedByCache);
        }

        [TestMethod]
        public async Task GiveAccessToPlant_CallsCorrectMethods()
        {
            //Arrange
            const string plantId = "somePlantId";
            const long personId = 2;
            const long projectId = 15;

            var (service, personProjectRepository, projectRepository, _, _, _, _, _, personService, personCreatedByCache) = Factory();

            projectRepository.Setup(pr => pr.GetParentProjectsByPlant(plantId))
                .Returns(Task.FromResult(new List<Project> { new Project { PlantId = plantId, ProjectId = projectId } }));

            //Act
            await service.GiveProjectAccessToPlantAsync(personId, plantId);

            //Assert
            personProjectRepository.Verify(ppr => ppr.AddAsync(projectId, personId, personCreatedByCache.Id), Times.Once);

        }

        [TestMethod]
        public async Task RemoveAccessToPlant_CallsCorrectMethods()
        {
            //Arrange
            const string plantId = "somePlantId";
            const long personId = 2;
            const long projectId = 4;

            var (service, personProjectRepository, projectRepository, _, _, _, _, _, personService, _) = Factory();

            personProjectRepository.Setup(ppr => ppr.VoidPersonProjects(plantId, personId))
                .Returns(new List<PersonProject>
                            { new PersonProject(projectId, personId, 123)
                                {
                                    Project = new Project { PlantId = plantId, ProjectId = projectId }
                                }
                             }
                );
            personProjectRepository.Setup(ppr => ppr.PersonHasNoAccess(personId))
                .Returns(Task.FromResult(true));

            //Act
            await  service.RemoveAccessToPlant(personId, plantId);

            //Assert
            personProjectRepository.Verify(ppr => ppr.VoidPersonProjects(plantId, personId), Times.Once);
        }
    }
}