using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PROJECT")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class Project
    {
        [Column("PROJECT_ID")]
        public long ProjectId { get; set; }

        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set; } = null!;

        [Column("ISVOIDED")]
        public bool IsVoided { get; set; }

        [Column("PARENT_PROJECT_ID")]
        public long? ParentProjectId { get; set; }

        [Column("ISMAINPROJECT")]
        public bool IsMainProject { get; set; }

        public virtual Plant? Plant { get; set; }
    }
}
