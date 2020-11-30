using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Constants;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    public class PersonProjectHistoryOperationConfiguration : IEntityTypeConfiguration<PersonProjectHistoryOperation>
    {
        public void Configure(EntityTypeBuilder<PersonProjectHistoryOperation> builder)
        {
            builder.HasKey(ppho => ppho.Id);
            builder.HasOne(ppho => ppho.PersonProjectHistory)
                .WithMany(pph => pph.PersonProjectHistoryOperations)
                .HasForeignKey(ppho => ppho.PersonProjectHistoryId);
            
            builder.ToTable("PERSONPROJECT_HISTORY_OP");

            builder.Property(ppho => ppho.Id)
                .HasColumnName("PERSONPROJECT_HISTORY_OP_ID")
                .ValueGeneratedOnAdd()
                .HasValueGenerator((_, __) => new SequenceValueGenerator(PersonProjectHistoryOperationConstants.Sequence));

            builder.Property(ppho => ppho.OperationType).HasColumnName("OPERATION_TYPE");
            builder.Property(ppho => ppho.OldValue).HasColumnName("OLD_VALUE");
            builder.Property(ppho => ppho.NewValue).HasColumnName("NEW_VALUE");
            builder.Property(ppho => ppho.FieldName).HasColumnName("FIELD_NAME");
            builder.Property(ppho => ppho.ProjectId).HasColumnName("PROJECT_ID");
            builder.Property(ppho => ppho.PersonId).HasColumnName("PERSON_ID");
            builder.Property(ppho => ppho.UpdatedByUser).HasColumnName("LAST_UPDATEDBYUSER");
            builder.Property(ppho => ppho.PersonProjectHistoryId).HasColumnName("PERSONPROJECT_HISTORY_ID");
        }
    }
}
