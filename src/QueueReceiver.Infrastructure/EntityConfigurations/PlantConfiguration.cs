using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class PlantConfiguration : IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.ToTable("PROJECTSCHEMA");
            builder.Property(plant => plant.PlantId).HasColumnName("PROJECTSCHEMA");
            builder.Property(plant => plant.AffiliateGroupId).HasColumnName("AFFILIATEGROUPID");
            builder.Property(plant => plant.InternalGroupId).HasColumnName("INTERNALGROUPID");
        }
    }
}
