using System;
using System.Collections.Generic;

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

        public string? Reconcile { get; set; }

        public string? ReconcilePlant { get; set; }

        public string? Oid { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MobilePhoneNumber { get; set; }

        public bool IsVoided { get; set; }

        public long? CreatedById { get; set; }

        public long? UpdatedById { get; set; }

        public string? UpdatedByUser { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual IEnumerable<PersonProject> PersonProjects { get; set; }
    }
}
