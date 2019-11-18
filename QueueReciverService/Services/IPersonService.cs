using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public interface IPersonService
    {
        ValueTask<(Person, bool)> FindOrCreate(string userOid, bool shouldCreate);
    }
}