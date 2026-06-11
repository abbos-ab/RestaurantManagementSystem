using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restaurant.Infrastructure.Persistence;

namespace Restaurant.Infrastructure.Jobs;

public sealed class RemoveExpiredRefreshTokens
{
    private readonly AppDbContext _context;
    private readonly ILogger<RemoveExpiredRefreshTokens> _logger;

    public RemoveExpiredRefreshTokens(AppDbContext context, ILogger<RemoveExpiredRefreshTokens> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            var tokens = await _context
                .RefreshTokens
                .Where(x => x.ExpiresAt <= DateTime.UtcNow)
                .ExecuteDeleteAsync();

            _logger.LogInformation("Удалено {RowsAffected} устаревших refresh токенов.", tokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении устаревших refresh токенов.");
        }
    }
}