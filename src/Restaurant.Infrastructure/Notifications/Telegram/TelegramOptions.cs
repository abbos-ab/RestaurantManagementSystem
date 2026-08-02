namespace Restaurant.Infrastructure.Notifications.Telegram;

public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    public string BotToken { get; set; } = string.Empty;

    public string ChatId { get; set; } = string.Empty;

    public bool Enabled { get; set; }
}