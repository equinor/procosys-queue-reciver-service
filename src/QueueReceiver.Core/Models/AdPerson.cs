namespace QueueReceiver.Core.Models
{
    public class AdPerson
    {
        public AdPerson(string oid, string username, string email)
        {
            Oid = oid;
            Username = username;
            Email = email;
        }

        public string Oid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
    }
}
