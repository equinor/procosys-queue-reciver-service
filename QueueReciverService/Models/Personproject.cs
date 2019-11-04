using System;

namespace QueueReciverService.Models
{
    public class Personproject
    {
        public int ProjectId { get; set; }
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
