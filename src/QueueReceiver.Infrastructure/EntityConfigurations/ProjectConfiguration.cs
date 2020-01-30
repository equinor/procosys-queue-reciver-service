using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("PROJECT");
            builder.Property(p => p.ProjectId).HasColumnName("PROJECT_ID");
            builder.Property(p => p.PlantId).HasColumnName("PROJECTSCHEMA");
            builder.Property(p => p.IsVoided).HasColumnName("ISVOIDED")
                .HasConversion(
                    b => b ? 'Y' : 'N',
                    c => c.Equals('Y'));
            builder.Property(p => p.ProjectId).HasColumnName("PARENT_PROJECT_ID");
            builder.Property(p => p.IsMainProject).HasColumnName("ISMAINPROJECT")
                .HasConversion(
                b => b ? 'Y' : 'N',
                c => c.Equals('Y'));

            builder.HasOne(project => project.Plant)
                .WithMany();
        }
    }
}
