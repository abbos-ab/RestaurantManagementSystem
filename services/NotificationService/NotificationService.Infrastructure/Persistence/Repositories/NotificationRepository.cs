using Ardalis.Specification.EntityFrameworkCore;
using NotificationService.Application.Features.Notifications.Repositories;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence.Repositories;

internal sealed class NotificationRepository(NotificationDbContext dbContext)
    : RepositoryBase<Notification>(dbContext), INotificationRepository; 