using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.EntityConfiguration
{
    public class PersonRestrictionRoleConfiguration : IEntityTypeConfiguration<PersonRestrictionRole>
    {

        public void Configure(EntityTypeBuilder<PersonRestrictionRole> builder)
        {
            builder.HasKey(prr => new { prr.PlantId, prr.RestrictionRole, prr.PersonId });

        }
    }
}
