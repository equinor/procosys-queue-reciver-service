using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Interfaces;
using QueueReceiver.Core.Models;
using System;

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
                UpdatedByUserName = PersonProjectHistoryConstants.UpdatedBy
            };

            return personProjectHistory;
        }

        public void LogAddAccess(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogInsert(personId, personProjectHistory, projectId, "INSERT");

        public void LogDefaultUserGroup(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogUpdate(personId, personProjectHistory, projectId, "User role", "Read", "N", "Y");

        public void LogUnvoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogUpdate(personId, personProjectHistory, projectId, "UPDATE", "ISVOIDED", "Y", "N");

        public void LogVoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId)
            => LogUpdate(personId, personProjectHistory, projectId, "UPDATE", "ISVOIDED", "N", "Y");

        private void LogInsert(long personId, PersonProjectHistory personProjectHistory, long projectId, string operationType)
        {
            var ppho = new PersonProjectHistoryOperation(
                operationType,
                projectId,
                personId,
                PersonProjectHistoryConstants.UpdatedBy,
                personProjectHistory);

            personProjectHistory.PersonProjectHistoryOperations.Add(ppho);
        }

        private void LogUpdate(long personId, PersonProjectHistory personProjectHistory, long projectId,
                                      string operationType, string fieldName, string oldValue, string newValue)
        {
            var ppho = new PersonProjectHistoryOperation(
                operationType,
                projectId,
                personId,
                PersonProjectHistoryConstants.UpdatedBy,
                personProjectHistory) 
            {
                OldValue = oldValue,
                NewValue = newValue,
                FieldName = fieldName
            };

            personProjectHistory.PersonProjectHistoryOperations.Add(ppho);
        }
    }
}