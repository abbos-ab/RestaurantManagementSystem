using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class NotificationRepository(AppDbContext dbContext)
    : RepositoryBase<Notification>(dbContext), INotificationRepository; 