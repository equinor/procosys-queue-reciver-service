using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("USERGROUP")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class UserGroup
    {
        [Column("ID")]
        public long Id { get; set; }

        [Column("NAME")]
        public string? Name { get; set; }
    }
}
