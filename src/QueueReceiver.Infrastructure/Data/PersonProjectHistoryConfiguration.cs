using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.Data
{
    public class PersonProjectHistoryConfiguration : IEntityTypeConfiguration<PersonProjectHistory>
    {
        public void Configure(EntityTypeBuilder<PersonProjectHistory> builder)
        {
            builder.Property(pph => pph.Id).ForOracleUseSequenceHiLo("SEQ_PERSONPROJECT_HISTORY");

            builder.HasKey(pph => pph.Id);
            builder.HasMany(pph => pph.PersonProjectHistoryOperations);
        }
    }
}
