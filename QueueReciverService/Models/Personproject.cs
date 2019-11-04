using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReciverService.Models
{
    [Table("PERSONPROJECT")]
    public class Personproject
    {
        [Column("PROJECT_ID")]
        public int ProjectId { get; set; }

        [Column("PERSON_ID")]
        public int PersonId { get; set; }
        public DateTime? Updatedat { get; set; }
        public int? UpdatedbyId { get; set; }
        public DateTime Createdat { get; set; }
        public int CreatedbyId { get; set; }
        public string Isvoided { get; set; }
        public string IsDefaultProject { get; set; }
        public string ShowInLogin { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastUpdatedbyuser { get; set; }
    }
}
