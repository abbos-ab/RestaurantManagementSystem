using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Persistence;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationDbContext _dbContext;

    public NotificationsController(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("user/{userId:long}")]
    public async Task<IActionResult> GetNotificationsByUserId(long userId)
    {
        var notifications = await _dbContext.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(notifications);
    }
}