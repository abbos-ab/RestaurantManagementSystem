namespace Restaurant.Mediator.Helper.Settings;

public sealed class UserTokenProviderSettings
{
    public required string Name { get; set; } = "UserTokenProvider";

    public required TimeSpan TokenLifeTime { get; set; } = TimeSpan.FromMinutes(10);
}
