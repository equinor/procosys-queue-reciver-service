using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantRepository
    {
        Task<string?> GetPlantIdByOidAsync(string plantOid);
        IEnumerable<string> GetAllInternalAndAffiliateOids();
    }
}
