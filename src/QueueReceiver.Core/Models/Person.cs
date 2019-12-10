using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSON")]
    public class Person
    {
        public Person(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        [Column("PERSON_ID")]
        public long Id { get; set; }
        [Column("AZURE_OID")]
        public string? Oid { get; set; }
        [Column("USERNAME")]
        public string UserName { get; set; }
        [Column("EMAILADDRESS")]
        public string Email { get; set; }
        [Column("FIRSTNAME")]
        public string? FirstName { get; set; }
        [Column("LASTNAME")]
        public string? LastName { get; set; }
    }
}
