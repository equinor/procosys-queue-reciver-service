using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONRESTRICTIONROLE")]//TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class PersonRestrictionRole
    {
        public PersonRestrictionRole(string plantId, string restrictionRole, long personId) // TODO: Remove some property setters?
        {
            PlantId = plantId;
            RestrictionRole = restrictionRole;
            PersonId = personId;
        }

        [Column("RESTRICTIONROLE")]
        public string RestrictionRole { get; set; }

        [Column("PERSON_ID")]
        public long PersonId { get; set; }

        [Column("PROJECTSCHEMA")]
        public string PlantId { get; set; }
    }
}