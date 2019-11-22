using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiverService.Models
{
    [Table("PROJECT")]
    public class Project
    {
        [Column("PROJECT_ID")]
        public long ProjectId { get; set; }

        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set; } = null!;

        [Column("ISVOIDED")]
        public bool IsVoided { get; set; }

        public virtual Plant? Plant { get; set; }
    }
}
