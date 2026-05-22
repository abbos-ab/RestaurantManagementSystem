using Auth.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Restaurant.Infrastructure.Persistence.Configurations;

internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}
