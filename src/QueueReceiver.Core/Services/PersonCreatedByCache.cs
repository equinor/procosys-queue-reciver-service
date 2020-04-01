namespace QueueReceiver.Core.Services
{
    public class PersonCreatedByCache
    {
        public PersonCreatedByCache(long personCreatedById)
        {
            Id = personCreatedById;
        }

        public long Id { get; set; }

        public string? Username { get; set; }
    }
}
