using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECT_HISTORY_OP")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class PersonProjectHistoryOperation
    {

        public PersonProjectHistoryOperation() //TODO: If this is for the ORM, at least make it protected. It also complains that there are uninitialized non-nullable properties in the class
        {
        }

        public PersonProjectHistoryOperation(
            string operationType,
            long projectId,
            long personId,
            string updatedByUser,
            PersonProjectHistory personProjectHistory) // TODO: Remove some property setters?
        {
            OperationType = operationType;
            ProjectId = projectId;
            PersonId = personId;
            UpdatedByUser = updatedByUser;
            PersonProjectHistory = personProjectHistory;
        }

        [Column("PERSONPROJECT_HISTORY_OP_ID")]
        public long Id { get; set; }

        [Column("OPERATION_TYPE")]
        public string OperationType { get; set; }

        [Column("OLD_VALUE")]
        public string? OldValue { get; set; }

        [Column("NEW_VALUE")]
        public string? NewValue { get; set; }

        [Column("FIELD_NAME")]
        public string? FieldName { get; set; }

        [Column("PROJECT_ID")]
        public long ProjectId { get; set; }

        [Column("PERSON_ID")]
        public long PersonId { get; set; }

        [Column("LAST_UPDATEDBYUSER")]
        public string UpdatedByUser { get; set; }

        [Column("PERSONPROJECT_HISTORY_ID")]
        public long PersonProjectHistoryId { get; set; }

        public PersonProjectHistory PersonProjectHistory { get; set; }
    }
}
