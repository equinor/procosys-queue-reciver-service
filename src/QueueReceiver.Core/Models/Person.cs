
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    public class Person
    {
        public Person(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        public long Id { get; set; }

        public string UserName { get; }

        public string Email { get; }

        public bool Reconcile { get; set; }

        public string? Oid { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MobilePhoneNumber { get; set; }

        public virtual IEnumerable<PersonProject> PersonProjects { get; set; }
    }
}
