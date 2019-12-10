using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.Data
{
    public class RestrictionRoleConfiguration : IEntityTypeConfiguration<RestrictionRole>
    {
        public void Configure(EntityTypeBuilder<RestrictionRole> builder)
        {
            builder.HasKey(rr => new { rr.PlantId, rr.RestrictionRoleId });
        }
    }
}