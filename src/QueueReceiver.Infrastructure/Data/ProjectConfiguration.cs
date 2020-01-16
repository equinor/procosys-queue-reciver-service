using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.Data
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasOne(project => project.Plant)
                .WithMany();

            builder.Property(p => p.IsVoided)
                .HasConversion(
                    b => b ? 'Y' : 'N',
                    c => c.Equals('Y'));

            builder.Property(p => p.IsMainProject)
                .HasConversion(
                b => b ? 'Y' : 'N',
                c => c.Equals('Y'));
        }
    }
}
