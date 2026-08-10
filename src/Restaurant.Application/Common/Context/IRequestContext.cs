namespace Restaurant.Application.Common.Context;

public interface IRequestContext
{
    long? UserId { get; }

    string? IpAddress { get; }

    string? UserAgent { get; }
}