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

        public PrivilegeService(IRestrictionRoleRepository restrictionRoleRepository,
            IPersonRestrictionRoleRepository personRestrictionRoleRepository,
            IUserGroupRepository userGroupRepository,
            IPersonUserGroupRepository personUserGroupRepository)
        {
            _restrictionRoleRepository = restrictionRoleRepository;
            _personRestrictionRoleRepository = personRestrictionRoleRepository;
            _userGroupRepository = userGroupRepository;
            _personUserGroupRepository = personUserGroupRepository;
        }

        public async Task GivePrivlieges(string plantId, long personId)
        {
            var userGroupId = await _userGroupRepository.FindIdByUserGroupName(PersonProjectConstants.DefaultUserGroup);
            await _personUserGroupRepository.AddIfNotExistAsync(userGroupId, plantId, personId);

            var restrictionRole = await _restrictionRoleRepository.FindRestrictionRole(PersonProjectConstants.DefaultRestrictionRole, plantId);
            await _personRestrictionRoleRepository.AddIfNotExistAsync(plantId, restrictionRole, personId);
        }
    }
}
