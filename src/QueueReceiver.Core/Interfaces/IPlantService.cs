using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantService
    {
        Task<string?> GetPlantIdAsync(string plantOid);
        IEnumerable<string> GetAllGroupOids();
    }
}