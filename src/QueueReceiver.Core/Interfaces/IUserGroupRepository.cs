using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IUserGroupRepository
    {
        Task<long> FindIdByUserGroupNameAsync(string name);
    }
}
