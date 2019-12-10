using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONRESTRICTIONROLE")]
    public class PersonRestrictionRole
    {
        public PersonRestrictionRole(string plantId, string restrictionRole, long personId)
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