using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfiguration
{
    public class PersonProjectHistoryConfiguration : IEntityTypeConfiguration<PersonProjectHistory>
    {
        public void Configure(EntityTypeBuilder<PersonProjectHistory> builder)
        {
            builder.Property(pph => pph.Id)
                .ForOracleUseSequenceHiLo(PersonProjectHistoryConstants.Sequence);

            builder.HasKey(pph => pph.Id);
            builder.HasMany(pph => pph.PersonProjectHistoryOperations);
        }
    }
}
