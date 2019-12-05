using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueReceiver.Core.Models;

namespace QueueReceiver.Infrastructure.Data
{
    public class PersonUserGroupConfiguration : IEntityTypeConfiguration<PersonUserGroup>
    {
        public void Configure(EntityTypeBuilder<PersonUserGroup> builder)
        {
            builder.HasKey(pug => new { pug.PlantId, pug.PersonId, pug.UserGroupId });
        }
    }
}
