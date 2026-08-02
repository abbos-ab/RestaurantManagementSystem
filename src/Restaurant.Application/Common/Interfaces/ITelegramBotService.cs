namespace Restaurant.Application.Common.Interfaces;

public interface ITelegramBotService
{
    Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
}