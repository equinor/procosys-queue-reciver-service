using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPlantRepository
    {
        Task<string?> GetPlantIdByOid(string plantOid);
        IEnumerable<string> GetAllInernalAndAffiliateOids();
    }
}
