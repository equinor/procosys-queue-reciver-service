using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Interfaces;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Services
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IPersonUserGroupRepository _personUserGroupRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IPersonRestrictionRoleRepository _personRestrictionRoleRepository;
        private readonly IRestrictionRoleRepository _restrictionRoleRepository;
        private readonly PersonCreatedByCache _personCreatedByCache;

        public PrivilegeService(IRestrictionRoleRepository restrictionRoleRepository,
            IPersonRestrictionRoleRepository personRestrictionRoleRepository,
            IUserGroupRepository userGroupRepository,
            IPersonUserGroupRepository personUserGroupRepository, 
            PersonCreatedByCache personCreatedByCache)
        {
            _restrictionRoleRepository = restrictionRoleRepository;
            _personRestrictionRoleRepository = personRestrictionRoleRepository;
            _userGroupRepository = userGroupRepository;
            _personUserGroupRepository = personUserGroupRepository;
            _personCreatedByCache = personCreatedByCache;
        }

        public async Task GivePrivilegesAsync(string plantId, long personId)
        {
            var userGroupId = await _userGroupRepository.FindIdByUserGroupNameAsync(PersonProjectConstants.DefaultUserGroup);
            await _personUserGroupRepository.AddIfNotExistAsync(userGroupId, plantId, personId, _personCreatedByCache.Id);

            var restrictionRole = await _restrictionRoleRepository.FindRestrictionRoleAsync(PersonProjectConstants.DefaultRestrictionRole, plantId);
            await _personRestrictionRoleRepository.AddIfNotExistAsync(plantId, restrictionRole, personId);
        }
    }
}
