using Microsoft.Extensions.Logging;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System.Threading.Tasks;
using QueueReceiver.Core.Properties;
using System.Collections.Generic;
using System.Linq;
using System;

namespace QueueReceiver.Core.Services
{
    public class AccessService : IAccessService
    {
        private readonly IPersonService _personService;
        private readonly IPersonProjectService _personProjectService;
        private readonly IPlantService _plantService;
        private readonly ILogger<AccessService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PersonCreatedByCache _personCreatedByCache;

        public AccessService(
            IPersonService personService,
            IPersonProjectService personProjectService,
            IPlantService plantService,
            ILogger<AccessService> logger,
            IUnitOfWork unitOfWork, 
            PersonCreatedByCache personCreatedByCache)
        {
            _personService = personService;
            _personProjectService = personProjectService;
            _plantService = plantService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _personCreatedByCache = personCreatedByCache;
        }

        public async Task HandleRequestAsync(AccessInfo accessInfo)
        {
            string? plantId = await _plantService.GetPlantIdAsync(accessInfo.PlantOid);

            if (plantId == null)
            {
                _logger.LogInformation(Resources.GroupDoesNotExist);
                return;
            }

            if (MessageHasNoRelevantData(accessInfo))
            {
                return;
            }

            // Set person CreatedBy cache
            await _personService.SetPersonCreatedByCache();

            _logger.LogInformation($"Updating access for {accessInfo.Members.Count} members to plant {plantId}");

            await UpdateMemberInfo(accessInfo.Members, plantId);

            await UpdateMemberAccess(accessInfo.Members, plantId);

            await UpdateMemberVoidedStatus(accessInfo.Members);
        }

        public async Task UpdateMemberInfo(List<Member> members, string plantId)
        {
            foreach (var member in members)
            {
                if (member.ShouldVoid)
                {
                    await _personService.UpdateWithOidIfNotFound(member.UserOid);
                }
                else
                {
                    await _personService.CreateIfNotExist(member.UserOid, plantId);
                }                
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMemberAccess(List<Member> members, string plantId)
        {
            foreach (var member in members)
            {
                if (member.ShouldVoid)
                {
                    await RemoveAccess(member.UserOid, plantId);
                }
                else
                {
                    await GiveAccess(member.UserOid, plantId);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMemberVoidedStatus(List<Member> members)
        {
            foreach (var member in members)
            {
                await _personService.UpdateVoidedStatus(member.UserOid);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task RemoveAccess(string userOid, string plantId)
        {
            Person? person = await _personService.FindPersonByOidAsync(userOid);

            if (person == null)
            {
                _logger.LogInformation(Resources.PersonDoesNotExist);
                return;
            }

            _logger.LogInformation(Resources.RemoveAccess, person.Id, plantId);

            var hasChanges = await _personProjectService.RemoveAccessToPlant(person.Id, plantId);

            if (hasChanges)
            {
                person.UpdatedAt = DateTime.Now;
                person.UpdatedById = _personCreatedByCache.Id;
            }
        }

        private async Task GiveAccess(string userOid, string plantId)
        {
            Person? person = await _personService.FindPersonByOidAsync(userOid);

            if (person == null)
            {
                _logger.LogError(Resources.PersonWasNotFoundOrCreated, userOid);
                return;
            }

            _logger.LogInformation(Resources.AddAccess, person.Id, plantId);

            var hasChanges = await _personProjectService.GiveProjectAccessToPlantAsync(person.Id, plantId);

            if (hasChanges)
            {
                person.UpdatedAt = DateTime.Now;
                person.UpdatedById = _personCreatedByCache.Id;
            }
        }

        private static bool MessageHasNoRelevantData(AccessInfo accessInfo)
            => accessInfo.Members == null;
    }
}