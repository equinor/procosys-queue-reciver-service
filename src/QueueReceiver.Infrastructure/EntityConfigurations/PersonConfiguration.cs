﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("PERSON");
            builder.Property(p => p.Id).HasColumnName("PERSON_ID");
            builder.Property(p => p.UserName).HasColumnName("USERNAME");
            builder.Property(p => p.Email).HasColumnName("EMAILADDRESS");
            builder.Property(p => p.Oid).HasColumnName("AZURE_OID");
            builder.Property(p => p.FirstName).HasColumnName("FIRSTNAME");
            builder.Property(p => p.LastName).HasColumnName("LASTNAME");
        }
    }
}