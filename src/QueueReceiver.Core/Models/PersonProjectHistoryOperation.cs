namespace QueueReceiver.Core.Models
{
    public class PersonProjectHistoryOperation
    {
        public PersonProjectHistoryOperation(
            string operationType,
            long projectId,
            long personId,
            string updatedByUser,
            PersonProjectHistory personProjectHistory)
        {
            OperationType = operationType;
            ProjectId = projectId;
            PersonId = personId;
            UpdatedByUser = updatedByUser;
            PersonProjectHistory = personProjectHistory;
        }

        public long Id { get; set; }

        public string OperationType { get; internal set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? FieldName { get; set; }

        public long ProjectId { get; internal set; }

        public long PersonId { get; internal set; }

        public string UpdatedByUser { get; internal set; }

        public long PersonProjectHistoryId { get; set; }

        public PersonProjectHistory PersonProjectHistory { get; internal set; }

        #pragma warning disable CS8618 //ORM.
        protected PersonProjectHistoryOperation()
        {
        }
    }
}
