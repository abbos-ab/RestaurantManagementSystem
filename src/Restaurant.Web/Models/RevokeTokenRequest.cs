namespace Restaurant.Web.Models;

public sealed class RevokeTokenRequest
{
    public required string RefreshToken { get; init; }
}