using System.Collections.Generic;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Core.Interfaces
{
    public interface IPersonProjectHistoryService
    {
        void LogAddAccess(long personId, PersonProjectHistory personProjectHistory, long projectId);
        void LogDefaultUserGroup(long personId, PersonProjectHistory personProjectHistory, long projectId);
        void LogUnvoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId);
        void LogVoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId);
        PersonProjectHistory CreatePersonProjectHistory(long personId);
    }
}
