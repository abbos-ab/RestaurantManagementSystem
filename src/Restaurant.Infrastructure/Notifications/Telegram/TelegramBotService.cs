using Microsoft.Extensions.Options;
using Restaurant.Application.Common.Interfaces;
using Telegram.Bot;

namespace Restaurant.Infrastructure.Notifications.Telegram;

public sealed class TelegramBotService : ITelegramBotService
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly TelegramOptions _telegramBotOptions;

    public TelegramBotService(IOptions<TelegramOptions> options)
    {
        _telegramBotOptions = options.Value;
        _telegramBotClient = new TelegramBotClient(_telegramBotOptions.BotToken);
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        if (!_telegramBotOptions.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(_telegramBotOptions.ChatId))
            return;

        await _telegramBotClient.SendMessage(chatId: _telegramBotOptions.ChatId, text: message, cancellationToken: cancellationToken);
    }
}