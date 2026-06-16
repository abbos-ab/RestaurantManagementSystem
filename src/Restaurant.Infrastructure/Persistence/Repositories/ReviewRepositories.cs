using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class ReviewRepositories(AppDbContext dbContext) : RepositoryBase<Review>(dbContext), IReviewRepository;