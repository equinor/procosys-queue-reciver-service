using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("RESTRICTIONROLE")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class RestrictionRole
    {
        [Column("RESTRICTIONROLE")]
        public string RestrictionRoleId { get; set; } = null!;

        [Column("PROJECTSCHEMA")] 
        public string PlantId { get; set; } = null!;
    }
}