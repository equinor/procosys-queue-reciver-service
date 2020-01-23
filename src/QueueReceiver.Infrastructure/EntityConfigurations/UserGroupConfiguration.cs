using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.ToTable("USERGROUP");
            builder.Property(ug => ug.Id).HasColumnName("ID");
            builder.Property(ug => ug.Name).HasColumnName("NAME");
        }
    }
}
