using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECT_HISTORY")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class PersonProjectHistory
    {
        public PersonProjectHistory()
        {
            PersonProjectHistoryOperations = new List<PersonProjectHistoryOperation>();
        }

        public List<PersonProjectHistoryOperation> PersonProjectHistoryOperations { get; set; } // TODO: Remove setter

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