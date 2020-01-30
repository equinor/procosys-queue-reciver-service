namespace QueueReceiver.Core.Models
{
    public class PersonProject
    {
        public PersonProject(long projectId, long personId, long createdById)
        {
            ProjectId = projectId;
            PersonId = personId;
            CreatedById = createdById;
        }

        public long ProjectId { get; }

        public long PersonId { get; }

        public long CreatedById { get; }

        public bool IsVoided { get; set; }

        public virtual Project? Project { get; set; }
    }
}
