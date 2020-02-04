﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueReceiver.Core.Services;
using QueueReceiver.Infrastructure.Data;
using QueueReceiver.Infrastructure.Repositories;
using System.Threading.Tasks;
using QueueReceiver.Core.Models;
using QueueReceiver.Core.Interfaces;
using Moq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using QueueReceiver.Core.Constants;

namespace QueueReceiver.IntegrationTests
{
    [TestClass]
    public class AccessServiceTests
    {
        #region Setup Service
        public static (AccessService,Mock<IGraphService>) Factory(QueueReceiverServiceContext context)
        {
            var personRepository = new PersonRepository(context);
            var graphServiceMock = new Mock<IGraphService>();
            var personService = new PersonService(personRepository, graphServiceMock.Object);
            var settings = new DbContextSettings { PersonProjectCreatedId = 111 };
            var personProjectRepository = new PersonProjectRepository(context, settings);
            var projectRepository = new ProjectRepository(context);
            var personUserGroupRepository = new PersonUserGroupRepository(context,settings);
            var userGroupRepository = new  UserGroupRepository(context);
            var personRestrictionRoleRepository = new PersonRestrictionRoleRepository(context);
            var restrictionRoleRepository = new RestrictionRoleRepository(context);
            var privilegeService = new PrivilegeService(restrictionRoleRepository, personRestrictionRoleRepository, userGroupRepository, personUserGroupRepository);
            var personProjectHistoryRepository = new PersonProjectHistoryRepository(context);
            var plantRepository = new PlantRepository(context);
            var plantService = new PlantService(plantRepository);
            var loggerMock = new Mock<ILogger<AccessService>>();

            var personProjectService = new PersonProjectService(personProjectRepository, projectRepository, privilegeService, personProjectHistoryRepository);
            var service = new AccessService(personService, personProjectService, plantService, loggerMock.Object,context);

            return (service, graphServiceMock);
        }
        #endregion

        [TestMethod]
        public async Task HandleRequest_GivesAccess_WithCorrectInput()
        {
            const string plantId = "plantId";
            const string PlantOid = "plantOid";
            const string memberOid = "memberOid";

            var options = new DbContextOptionsBuilder<QueueReceiverServiceContext>()
                .UseInMemoryDatabase(databaseName: "Gives_access")
                .Options;

            using (var context = new QueueReceiverServiceContext(options))
            {
                context.Persons.Add(new Person("TestUser", "TestEmail") { Oid = memberOid });
                context.Plants.Add(new Plant { InternalGroupId = PlantOid, PlantId = plantId });
                context.Projects.Add(new Project { ParentProjectId = null, PlantId = plantId });
                context.UserGroups.Add(new UserGroup { Name = PersonProjectConstants.DefaultUserGroup });
                context.RestrictionRoles.Add(new RestrictionRole { PlantId = plantId, RestrictionRoleId = PersonProjectConstants.DefaultRestrictionRole });
                await context.SaveChangesAsync();
            }

            using (var context = new QueueReceiverServiceContext(options))
            {
                var (service, graphMock) = Factory(context);
                var accessInfo = new AccessInfo(PlantOid, new List<Member>() { new Member(memberOid, false) });
                await service.HandleRequest(accessInfo);
            }

            using (var context = new QueueReceiverServiceContext(options))
            {
              Assert.IsTrue(await context.PersonProjects.CountAsync() == 1);
            }
        }
    }
}