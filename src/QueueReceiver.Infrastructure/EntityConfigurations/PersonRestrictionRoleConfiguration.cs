using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class PersonRestrictionRoleConfiguration : IEntityTypeConfiguration<PersonRestrictionRole>
    {

        public void Configure(EntityTypeBuilder<PersonRestrictionRole> builder)
        {
            builder.HasKey(prr => new { prr.PlantId, prr.RestrictionRole, prr.PersonId });
            builder.ToTable("PERSONRESTRICTIONROLE");
            builder.Property(prr => prr.RestrictionRole).HasColumnName("RESTRICTIONROLE");
            builder.Property(prr => prr.PersonId).HasColumnName("PERSON_ID");
            builder.Property(prr => prr.PlantId).HasColumnName("PROJECTSCHEMA");
        }
    }
}
