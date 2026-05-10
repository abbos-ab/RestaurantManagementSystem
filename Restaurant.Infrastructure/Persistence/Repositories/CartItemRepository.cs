using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Carts.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class CartItemRepository(AppDbContext dbContext) : RepositoryBase<CartItem>(dbContext),ICartItemRepository;