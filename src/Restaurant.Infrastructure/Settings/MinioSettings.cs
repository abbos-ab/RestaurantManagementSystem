namespace Restaurant.Infrastructure.Settings;

public sealed class MinioSettings
{
    public required string Scheme { get; init; }

    public required string Endpoint { get; init; }

    public required string AccessKey { get; init; }

    public required string SecretKey { get; init; }

    public required string Region { get; init; }

    public required string BucketName { get; init; }

    public required bool Secure { get; init; }
}
