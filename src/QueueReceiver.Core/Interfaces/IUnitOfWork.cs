using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
