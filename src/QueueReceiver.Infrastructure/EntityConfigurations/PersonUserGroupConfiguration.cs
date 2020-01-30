using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class PersonUserGroupConfiguration : IEntityTypeConfiguration<PersonUserGroup>
    {
        public void Configure(EntityTypeBuilder<PersonUserGroup> builder)
        {
            builder.HasKey(pug => new { pug.PlantId, pug.PersonId, pug.UserGroupId });
            builder.ToTable("PERSONUSERGROUP");
            builder.Property(pug => pug.PersonId).HasColumnName("PERSON_ID");
            builder.Property(pug => pug.UserGroupId).HasColumnName("USERGROUP_ID");
            builder.Property(pug => pug.PlantId).HasColumnName("PROJECTSCHEMA");
            builder.Property(pug => pug.CreatedById).HasColumnName("CREATEDBY_ID");
        }
    }
}
