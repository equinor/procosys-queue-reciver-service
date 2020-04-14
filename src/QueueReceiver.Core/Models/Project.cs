
namespace QueueReceiver.Core.Models
{
    public class Project
    {
        public long ProjectId { get; set; }

        public string PlantId { get; set; } = null!;

        public bool IsVoided { get; set; }

        public long? ParentProjectId { get; set; }

        public bool IsMainProject { get; set; }

        public virtual Plant? Plant { get; set; }
    }
}
