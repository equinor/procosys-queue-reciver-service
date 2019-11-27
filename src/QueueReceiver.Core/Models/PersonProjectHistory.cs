using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECTHISTORY")]
    public class PersonProjectHistory
    {
        [Column("PERSONPROJECT_HISTORY_ID")]
        public long PersonProjectHistoryId { get; set; }
    }
}
