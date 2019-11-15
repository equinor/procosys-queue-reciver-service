﻿using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiverService.Models
{
    [Table("PERSON")]
    public class Person
    {
        [Column("PERSON_ID")]
        public int Id { get; set; }
        [Column("AZURE_OID")]
        public string Oid { get; set; }
        [Column("USERNAME")]
        public string UserName { get; set; }
        [Column("EMAILADDRESS")]
        public string Email { get; set; }
    }
}
