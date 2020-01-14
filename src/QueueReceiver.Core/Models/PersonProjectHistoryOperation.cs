using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECT_HISTORY_OP")]
    public class PersonProjectHistoryOperation
    {
        [Column("PERSONPROJECT_HISTORY_OP_ID")]
        public long Id { get; set; }

        [Column("OPERATION_TYPE")]
        public string OperationType { get; set; }

        [Column("OLD_VALUE")]
        public string OldValue { get; set; }

        [Column("NEW_VALUE")]
        public string NewValue { get; set; }

        [Column("FIELD_NAME")]
        public string FieldName { get; set; }

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
