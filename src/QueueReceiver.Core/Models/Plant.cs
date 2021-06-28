namespace QueueReceiver.Core.Models
{
    public class Plant
    {
        public string PlantId { get; set; } = null!;

        public string AffiliateGroupId { get; set; } = null!;

        public string InternalGroupId { get; set; } = null!;

        public bool IsVoided { get; set; }
    }
}
