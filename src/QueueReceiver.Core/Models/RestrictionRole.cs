using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("RESTRICTIONROLE")]
    public class RestrictionRole
    {
        [Column("RESTRICTIONROLE")]
        public string RestrictionRoleId { get; set; } = null!;

        [Column("PROJECTSCHEMA")] 
        public string PlantId { get; set; } = null!;
    }
}