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

        public string Oid { get; }
        public string Username { get; }
        public string Email { get; }
        public string GivenName { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public bool IsDeleted { get; set; }

    }
}
