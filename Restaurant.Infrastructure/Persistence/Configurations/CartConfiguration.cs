using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Cart");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.TotalPrice)
            .HasColumnType("decimal(18,2)");
        
        builder
            .HasMany(x => x.CartItems)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}