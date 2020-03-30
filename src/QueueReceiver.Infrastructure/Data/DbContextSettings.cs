namespace QueueReceiver.Infrastructure.Data
{
    public class DbContextSettings
    {
        public long PersonProjectCreatedId { get; set; }

        // Will be populated from DB
        public string? PersonProjectCreatedUsername { get; set; }
    }
}
