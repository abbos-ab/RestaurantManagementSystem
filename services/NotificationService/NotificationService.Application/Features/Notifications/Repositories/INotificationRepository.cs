using Ardalis.Specification;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Features.Notifications.Repositories;

public interface INotificationRepository : IRepositoryBase<Notification>;