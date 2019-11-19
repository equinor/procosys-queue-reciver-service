using System.Threading.Tasks;
using QueueReceiverService.Models;

namespace QueueReceiverService.Services
{
    public interface IPersonService
    {
        ValueTask<(Person person, bool success)> FindOrCreate(string userOid, bool shouldNotCreate=false);
    }
}