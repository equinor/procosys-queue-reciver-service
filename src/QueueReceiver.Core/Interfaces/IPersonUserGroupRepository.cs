using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonUserGroupRepository
    {
        Task AddIfNotExistAsync(long userGroupId, string plantId, long personId);
    }
}