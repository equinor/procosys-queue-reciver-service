using QueueReceiver.Core.Models;
using System;

namespace QueueReceiver.Core.Services
{
    public static class PersonProjectHistoryHelper
    {
        public static PersonProjectHistory CreatePersonProjectHistory(long updatedById, string? updatedByUserName)
        {
            var personProjectHistory = new PersonProjectHistory
            {
                UpdatedAt = DateTime.Now,
                UpdatedBy = updatedById,
                UpdatedByUserName = updatedByUserName
            };

            return personProjectHistory;
        }

        public static void LogAddAccess(long personId, PersonProjectHistory personProjectHistory, long projectId, string? updatedByUsername)
            => LogInsert(personId, personProjectHistory, projectId, "INSERT", updatedByUsername);

        public static void LogDefaultUserGroup(long personId, PersonProjectHistory personProjectHistory, long projectId, string? updatedByUsername)
            => LogUpdate(personId, personProjectHistory, projectId, "User role", "Read", "N", "Y", updatedByUsername);

        public static void LogUnvoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId, string? updatedByUsername)
            => LogUpdate(personId, personProjectHistory, projectId, "UPDATE", "ISVOIDED", "Y", "N", updatedByUsername);

        public static void LogVoidProjects(long personId, PersonProjectHistory personProjectHistory, long projectId, string? updatedByUsername)
            => LogUpdate(personId, personProjectHistory, projectId, "UPDATE", "ISVOIDED", "N", "Y", updatedByUsername);

        private static void LogInsert(
            long personId, 
            PersonProjectHistory personProjectHistory, 
            long projectId, 
            string operationType,
            string? updatedByUsername)
        {
            var ppho = new PersonProjectHistoryOperation(
                operationType,
                projectId,
                personId,
                updatedByUsername,
                personProjectHistory);

            personProjectHistory.PersonProjectHistoryOperations.Add(ppho);
        }

        private static void LogUpdate(
            long personId, 
            PersonProjectHistory personProjectHistory, 
            long projectId,
            string operationType, 
            string fieldName, 
            string oldValue, 
            string newValue,
            string? updatedByUsername)
        {
            var ppho = new PersonProjectHistoryOperation(
                operationType,
                projectId,
                personId,
                updatedByUsername,
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