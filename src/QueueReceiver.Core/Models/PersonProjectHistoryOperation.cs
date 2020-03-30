namespace QueueReceiver.Core.Models
{
    public class PersonProjectHistoryOperation
    {
        public PersonProjectHistoryOperation(
            string operationType,
            long projectId,
            long personId,
            PersonProjectHistory personProjectHistory)
        {
            OperationType = operationType;
            ProjectId = projectId;
            PersonId = personId;
            PersonProjectHistory = personProjectHistory;
        }

        public long Id { get; set; }

        public string OperationType { get; internal set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? FieldName { get; set; }

        public long ProjectId { get; internal set; }

        public long PersonId { get; internal set; }

        public string? UpdatedByUser { get; set; }

        public long PersonProjectHistoryId { get; set; }

        public PersonProjectHistory PersonProjectHistory { get; internal set; }

        #pragma warning disable CS8618 //Entity framework requires an empty constructor.
        protected PersonProjectHistoryOperation()
        {
        }
    }
}
