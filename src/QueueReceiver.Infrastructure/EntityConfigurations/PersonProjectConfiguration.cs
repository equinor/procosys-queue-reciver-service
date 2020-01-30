using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
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

            builder.ToTable("PERSONPROJECT");
            builder.Property(pp => pp.ProjectId).HasColumnName("PROJECT_ID");
            builder.Property(pp => pp.PersonId).HasColumnName("PERSON_ID");
            builder.Property(pp => pp.CreatedById).HasColumnName("CREATEDBY_ID");
            builder.Property(pp => pp.IsVoided).HasColumnName("ISVOIDED");
        }
    }
}
