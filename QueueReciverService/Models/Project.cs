using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiverService.Models
{
    [Table("PROJECT")]
    public class Project
    {
        [Column("PROJECT_ID")]
        public long ProjectId { get; set; }
                 
        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set;}

        [Column("ISVOIDED")]
        public bool IsVoided { get; set; }

        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }
    }
}
