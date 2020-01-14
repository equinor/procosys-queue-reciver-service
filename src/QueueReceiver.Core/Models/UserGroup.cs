using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("USERGROUP")]
    public class UserGroup
    {
        [Column("ID")]
        public long Id { get; set; }

        [Column("NAME")]
        public string? Name { get; set; }
    }
}
