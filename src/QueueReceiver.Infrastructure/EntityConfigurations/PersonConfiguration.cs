using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("PERSON");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("PERSON_ID").ForOracleUseSequenceHiLo(PersonConstants.Sequence);
            builder.Property(p => p.UserName).HasColumnName("USERNAME");
            builder.Property(p => p.Email).HasColumnName("EMAILADDRESS");
            builder.Property(p => p.Oid).HasColumnName("AZURE_OID");
            builder.Property(p => p.FirstName).HasColumnName("FIRSTNAME");
            builder.Property(p => p.LastName).HasColumnName("LASTNAME");
            builder.Property(p => p.MobilePhoneNumber).HasColumnName("MOBILEPHONENO");
            builder.Property(p => p.Reconcile).HasColumnName("RECONCILE");
            builder.Property(p => p.ReconcileProjectschema).HasColumnName("RECONCILE_PROJECTSCHEMA");
            builder.Property(p => p.CreatedById).HasColumnName("CREATEDBY_ID");
            builder.Property(p => p.UpdatedById).HasColumnName("UPDATEDBY_ID");
            builder.Property(p => p.UpdatedAt).HasColumnName("UPDATEDAT");

            builder.HasMany(p => p.PersonProjects);

            builder.Property(p => p.IsVoided).HasColumnName("ISVOIDED")
                .HasConversion(
                    b => b ? 'Y' : 'N',
                    c => c.Equals('Y'));
        }
    }
}