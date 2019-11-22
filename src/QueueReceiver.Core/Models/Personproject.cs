using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECT")]
    public class PersonProject
    {
        public PersonProject(long personId, long projectId)
        {
            ProjectId = projectId;
            PersonId = personId;
        }

        [Column("PROJECT_ID")]
        public long ProjectId { get; set; }

        [Column("PERSON_ID")]
        public long PersonId { get; set; }

        [Column("CREATEDBY_ID")]
        public long CreatedById { get; set; }

        public virtual Project? Project { get; set; }
    }
}
