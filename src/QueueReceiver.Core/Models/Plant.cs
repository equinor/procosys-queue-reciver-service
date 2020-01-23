using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PROJECTSCHEMA")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class Plant
    {
        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set; } = null!;

        [Column("AFFILIATEGROUPID")]
        public string AffiliateGroupId { get; set; } = null!;

        [Column("INTERNALGROUPID")]
        public string InternalGroupId { get; set; } = null!;
    }
}
