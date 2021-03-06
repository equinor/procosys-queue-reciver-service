﻿namespace QueueReceiver.Core.Models
{
    public class Plant
    {
        public string PlantId { get; set; } = null!;

        public string? AffiliateGroupId { get; set; }

        public string? InternalGroupId { get; set; }
        
        public bool IsVoided { get; set; }
    }
}
