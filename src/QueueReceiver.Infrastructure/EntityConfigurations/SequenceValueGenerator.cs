using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace QueueReceiver.Infrastructure.EntityConfigurations
{
    internal class SequenceValueGenerator : ValueGenerator<long>
    {
        private readonly string _sequenceName;

        public SequenceValueGenerator(string sequenceName)
        {
            _sequenceName = sequenceName;
        }

        public override long Next(EntityEntry entry)
        {
            using var command = entry.Context.Database.GetDbConnection().CreateCommand();

            command.CommandText = $"SELECT {_sequenceName}.NEXTVAL FROM DUAL";
            
            entry.Context.Database.OpenConnection();
            
            using var reader = command.ExecuteReader();
            
            reader.Read();
            return reader.GetInt64(0);
        }

        public override bool GeneratesTemporaryValues => false;
    }
}
