using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasMaxLength(100);
        
        builder
            .Property(x => x.PhoneNumber)
            .HasConversion(
                x => x.Value,
                x => new PhoneNumber(x));

        builder
            .HasIndex(x => x.PhoneNumber)
            .IsUnique();
        
        builder
            .HasMany(x => x.Groups)
            .WithMany(x => x.Users)
            .UsingEntity<UserGroupRelation>(
                j =>
                {
                    j.HasKey(x => new { x.UserId, x.GroupId });

                    j.HasOne(x => x.User)
                        .WithMany()
                        .HasForeignKey(x => x.UserId);

                    j.HasOne(x => x.Group)
                        .WithMany()
                        .HasForeignKey(x => x.GroupId);
                });
    }
}