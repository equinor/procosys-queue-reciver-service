using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IRestrictionRoleRepository
    {
        Task<string> FindRestrictionRole(string name, string plant);
    }
}
