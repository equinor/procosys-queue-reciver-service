﻿using System.ComponentModel.DataAnnotations.Schema;

namespace QueueReceiver.Core.Models
{
    [Table("PERSONPROJECT")]
    public class PersonProject
    {
        public PersonProject(long projectId, long personId, long createdById)
        {
            ProjectId = projectId;
            PersonId = personId;
            CreatedById = createdById;
        }

        [Column("PROJECT_ID")]
        public long ProjectId { get; set; }

        [Column("PERSON_ID")]
        public long PersonId { get; set; }

        [Column("CREATEDBY_ID")]
        public long CreatedById { get; set; }

        [Column("ISVOIDED")]
        public bool IsVoided { get; set; }

        public virtual Project? Project { get; set; }
    }
}