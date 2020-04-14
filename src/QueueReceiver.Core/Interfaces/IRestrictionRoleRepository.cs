using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IRestrictionRoleRepository
    {
        Task<string> FindRestrictionRoleAsync(string name, string plant);
    }
}
