using System.Collections.Generic;
using Restaurant.Application.Features.Carts.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Carts;

[Mapper]
public partial class CartMapper
{
    public partial CartDto Map(Cart cart);
    public partial List<CartDto> Map(List<Cart> carts);

    public partial CartItemDto Map(CartItem cartItem);
}