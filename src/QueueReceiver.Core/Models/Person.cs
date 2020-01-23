using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSON")] //TODO: I assume these annotations are for the ORM. There are already entityconfigurations in the infrastructure project that use the fluent api. Move this to the entity configuration files to keep all related configuration in one place.
    public class Person
    {
        public Person(string userName, string email) // TODO: If username and email are not supposed to be changed, remove the setters on the properties.
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
