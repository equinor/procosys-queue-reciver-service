
namespace QueueReceiver.Core.Models
{
    public class PersonUserGroup
    {
        public PersonUserGroup(long personId, long userGroupId, string plantId, long createdById)
        {
            PersonId = personId;
            UserGroupId = userGroupId;
            PlantId = plantId;
            CreatedById = createdById;
        }

        public long PersonId { get; }

        public long UserGroupId { get; }

        public string PlantId { get; }

        public long CreatedById { get; }
    }
}
