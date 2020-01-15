using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Core.Services
{
    public class PersonProjectHistoryService : IPersonProjectHistoryService
    {

        public PersonProjectHistory CreatePersonProjectHistory(long personId)
        {
            var personProjectHistory = new PersonProjectHistory()
            {
                UpdatedAt = DateTime.Now,
                UpdatedBy = personId,
                UpdatedByUserName = updatedBy
            };

            return personProjectHistory;
        }

        public void LogAddAccess(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogInsert(personId, personProjectHistory, projectId, "INSERT", "ACCESS SYNC");

        public void LogDefaultUserGroup(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogUpdate(personId, personProjectHistory, projectId, "User role", "Read", "N", "Y", "ACCESS SYNC");

        public void LogUnvoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogUpdate(personId, personProjectHistory, projectId, "UPDATE", "ISVOIDED", "Y", "N", "ACCESS SYNC");

        public void LogVoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogUpdate(personId, personProjectHistory, projectId, "UPDATE", "ISVOIDED", "N", "Y", "ACCESS SYNC");

        private void LogInsert(long personId, PersonProjectHistory personProjectHistory, long projectId, string operationType, string updatedBy)
        {
            var ppho = new PersonProjectHistoryOperation()
            {
                OperationType = operationType,
                ProjectId = projectId,
                PersonId = personId,
                UpdatedByUser = updatedBy,
                PersonProjectHistory = personProjectHistory
            };
            personProjectHistory.PersonProjectHistoryOperations.Add(ppho);
        }

        private void LogUpdate(long personId, PersonProjectHistory personProjectHistory, long projectId,
                                      string operationType, string fieldName, string oldValue, string newValue, string updatedBy)
        {
            var ppho = new PersonProjectHistoryOperation()
            {
                OperationType = operationType,
                OldValue = oldValue,
                NewValue = newValue,
                ProjectId = projectId,
                PersonId = personId,
                UpdatedByUser = updatedBy,
                PersonProjectHistory = personProjectHistory,
                FieldName = fieldName
            };
            personProjectHistory.PersonProjectHistoryOperations.Add(ppho);
        }
    }
}