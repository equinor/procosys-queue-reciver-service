using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonUserGroupRepository
    {
        Task AddAsync(long userGroupId, string plantId, long personId);
    }
}
