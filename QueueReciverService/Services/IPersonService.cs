using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public interface IPersonService
    {
        Task<Person> FindOrCreate(string userOid, bool shouldRemove=false);
    }
}