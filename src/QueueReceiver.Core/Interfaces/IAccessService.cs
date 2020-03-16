using QueueReceiver.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Core.Interfaces
{
    public interface IAccessService
    {
        Task HandleRequestAsync(AccessInfo accessInfo);
        Task UpdateMemberInfo(List<Member> members);
        Task UpdateMemberAccess(List<Member> members, string plantId);
    }
}
