using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReciverService.Models
{
    public class Project
    {
        [Column("PROJECT_ID"), ReadOnly(true)]
        public long ProjectId { get; set; }

        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set;}
    }
}
