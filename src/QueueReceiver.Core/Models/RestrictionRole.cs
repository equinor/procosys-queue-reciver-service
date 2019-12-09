using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("RESTRICTIONROLE")]
    public class RestrictionRole
    {
        [Column("RESTRICTIONROLE")]
        public string Id { get; set; }

        [Column("PROJECTSCHEMA")] 
        public string plantId { get; set; } = null!;
    }
}