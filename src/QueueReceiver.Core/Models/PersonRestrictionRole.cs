namespace QueueReceiver.Core.Models
{
    public class PersonRestrictionRole
    {
        public PersonRestrictionRole(string plantId, string restrictionRole, long personId)
        {
            PlantId = plantId;
            RestrictionRole = restrictionRole;
            PersonId = personId;
        }

        public string RestrictionRole { get; }

        public long PersonId { get; }

        public string PlantId { get; }
    }
}