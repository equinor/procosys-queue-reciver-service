namespace QueueReceiver.Core.Models
{
    public class AdPerson
    {
        public AdPerson(string oid, string username, string email) //TODO: Remove all property setters and accept all information in constructor. At least remove the setters for oid and username.
        {
            Oid = oid;
            Username = username;
            Email = email;
        }

        public string Oid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
    }
}
