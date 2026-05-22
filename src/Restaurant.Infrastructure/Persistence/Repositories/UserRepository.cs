using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : RepositoryBase<User>(dbContext), IUserRepository;