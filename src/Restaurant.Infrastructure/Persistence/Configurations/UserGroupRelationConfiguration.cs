using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

internal sealed class UserGroupRelationConfiguration : IEntityTypeConfiguration<UserGroupRelation>
{
    public void Configure(EntityTypeBuilder<UserGroupRelation> builder)
    {
        builder.HasKey(
            x => new
            {
                x.UserId,
                x.GroupId,
            }
        );
    }
}
