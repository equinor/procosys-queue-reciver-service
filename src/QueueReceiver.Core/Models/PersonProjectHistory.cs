using System;
using System.Collections.Generic;

namespace QueueReceiver.Core.Models
{
    public class PersonProjectHistory
    {
        public PersonProjectHistory()
        {
            PersonProjectHistoryOperations = new List<PersonProjectHistoryOperation>();
        }

        public List<PersonProjectHistoryOperation> PersonProjectHistoryOperations { get; }

        public long Id { get; set; }

        public long UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? UpdatedByUserName { get; set; }
    }
}