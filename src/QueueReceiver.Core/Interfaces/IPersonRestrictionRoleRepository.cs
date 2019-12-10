using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonRestrictionRoleRepository
    {
        Task AddIfNotExistAsync(string plantId, string restrictionRole, long personId);
    }
}
