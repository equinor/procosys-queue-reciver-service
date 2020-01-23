using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECT")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class PersonProject
    {
        public PersonProject(long projectId, long personId, long createdById) //TODO: If these properties are not supposed to be changed after object creation, remove property setters
        {
            ProjectId = projectId;
            PersonId = personId;
            CreatedById = createdById;
        }

        [Column("PROJECT_ID")]
        public long ProjectId { get; set; }

        [Column("PERSON_ID")]
        public long PersonId { get; set; }

        [Column("CREATEDBY_ID")]
        public long CreatedById { get; set; }

        [Column("ISVOIDED")]
        public bool IsVoided { get; set; }

        public virtual Project? Project { get; set; }
    }
}
