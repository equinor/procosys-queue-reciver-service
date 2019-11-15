﻿using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiverService.Models
{
    [Table("PROJECTSCHEMA")]
    public class Plant
    {
        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set; }

        [Column("AFFILIATEGROUPID")]
        public string AffiliateGroupId { get; set; }

        [Column("INTERNALGROUPID")]
        public string InternalGroupId { get; set; }

    }
}
