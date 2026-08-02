using Microsoft.AspNetCore.Http;
using Restaurant.Application.Common.Interfaces;

namespace Restaurant.Infrastructure.Notifications.Telegram;


public sealed class TelegramExceptionNotifier
    : IExceptionNotifier
{

    private readonly ITelegramBotService _telegramBotService;


    public TelegramExceptionNotifier(
        ITelegramBotService telegramBotService)
    {
        _telegramBotService = telegramBotService;
    }



    public async Task NotifyAsync(
        Exception exception,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {

        var message =
            TelegramMessageBuilder.Build(
                exception,
                context);


        await _telegramBotService.SendMessageAsync(
            message,
            cancellationToken);
    }
}