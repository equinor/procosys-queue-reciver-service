using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECT_HISTORY")]
    public class PersonProjectHistory
    {
        public PersonProjectHistory()
        {
            PersonProjectHistoryOperations = new List<PersonProjectHistoryOperation>();
        }

        public List<PersonProjectHistoryOperation> PersonProjectHistoryOperations { get; }

        [Column("PERSONPROJECT_HISTORY_ID")]
        public long Id { get; set; }

        [Column("UPDATEDBY_ID")]
        public long UpdatedBy { get; set; }

        [Column("UPDATEDAT")]
        public DateTime UpdatedAt { get; set; }

        [Column("LAST_UPDATEDBYUSER")]
        public string? UpdatedByUserName { get; set; }
    }
}