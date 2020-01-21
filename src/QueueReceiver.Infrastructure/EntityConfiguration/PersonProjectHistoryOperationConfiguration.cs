using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfiguration
{
    public class PersonProjectHistoryOperationConfiguration : IEntityTypeConfiguration<PersonProjectHistoryOperation>
    {
        public void Configure(EntityTypeBuilder<PersonProjectHistoryOperation> builder)
        {
            builder.Property(ppho => ppho.Id)
                .ForOracleUseSequenceHiLo(PersonProjectHistoryOperationConstants.Sequence);

            builder.HasKey(ppho => ppho.Id);
            builder.HasOne(ppho => ppho.PersonProjectHistory)
                .WithMany(pph => pph.PersonProjectHistoryOperations)
                .HasForeignKey(ppho => ppho.PersonProjectHistoryId);
        }
    }
}
