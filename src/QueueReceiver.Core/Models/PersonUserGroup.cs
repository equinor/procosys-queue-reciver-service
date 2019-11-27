using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONUSERGROUP")]
    public class PersonUserGroup
    {
        public PersonUserGroup(long personId, long userGroupId, string plantId, long createdById)
        {
            PersonId = personId;
            UserGroupId = userGroupId;
            PlantId = plantId;
            CreatedById = createdById;
        }

        [Column("PERSON_ID")]
        public long PersonId { get; set; }

        [Column("USERGROUP_ID")]
        public long UserGroupId { get; set; }

        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set; }

        [Column("CREATEDBY_ID")]
        public long CreatedById { get; set; }
    }
}
