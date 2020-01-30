using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class RestrictionRoleConfiguration : IEntityTypeConfiguration<RestrictionRole>
    {
        public void Configure(EntityTypeBuilder<RestrictionRole> builder)
        {
            builder.HasKey(rr => new { rr.PlantId, rr.RestrictionRoleId });

            builder.ToTable("RESTRICTIONROLE");
            builder.Property(rr => rr.RestrictionRoleId).HasColumnName("RESTRICTIONROLE");
            builder.Property(rr => rr.PlantId).HasColumnName("PROJECTSCHEMA");
        }
    }
}