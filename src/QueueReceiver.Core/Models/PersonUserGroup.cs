using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONUSERGROUP")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class PersonUserGroup
    {
        public PersonUserGroup(long personId, long userGroupId, string plantId, long createdById) //TODO: Remove some property setters?
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
