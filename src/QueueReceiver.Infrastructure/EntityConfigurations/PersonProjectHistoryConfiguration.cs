using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class PersonProjectHistoryConfiguration : IEntityTypeConfiguration<PersonProjectHistory>
    {
        public void Configure(EntityTypeBuilder<PersonProjectHistory> builder)
        {
            builder.Property(pph => pph.Id)
                .ForOracleUseSequenceHiLo(PersonProjectHistoryConstants.Sequence);

            builder.HasKey(pph => pph.Id);
            builder.HasMany(pph => pph.PersonProjectHistoryOperations);

            builder.ToTable("PERSONPROJECT_HISTORY");
            builder.Property(pph => pph.Id).HasColumnName("PERSONPROJECT_HISTORY_ID");
            builder.Property(pph => pph.UpdatedBy).HasColumnName("UPDATEDBY_ID");
            builder.Property(pph => pph.UpdatedAt).HasColumnName("UPDATEDAT");
            builder.Property(pph => pph.UpdatedByUserName).HasColumnName("LAST_UPDATEDBYUSER");
        }
    }
}
