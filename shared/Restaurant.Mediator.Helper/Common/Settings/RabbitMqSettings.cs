namespace Restaurant.Mediator.Helper.Common.Settings;

public sealed class RabbitMqSettings
{
    public required string Host { get; init; }

    public required string VirtualHost { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }
}