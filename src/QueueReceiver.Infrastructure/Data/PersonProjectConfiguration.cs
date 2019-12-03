using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.Data
{
    public class PersonProjectConfiguration : IEntityTypeConfiguration<PersonProject>
    {
        public void Configure(EntityTypeBuilder<PersonProject> builder)
        {
            builder.HasKey(pp => new { pp.ProjectId, pp.PersonId });
            builder.HasOne(pp => pp.Project)
                .WithMany()
                .HasForeignKey(pp => pp.ProjectId);

            builder.Property(p => p.IsVoided)
                .HasConversion(
                    b => b ? 'Y' : 'N',
                    c => c.Equals('Y'));
        }
    }
}
