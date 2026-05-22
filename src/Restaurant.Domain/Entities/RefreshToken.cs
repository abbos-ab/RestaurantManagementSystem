namespace Restaurant.Domain.Entities;

public sealed class RefreshToken
{
    public long Id { get; set; }

    public required string Token { get; set; }

    public required long UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedDate { get; set; }

    public bool IsRevoked => RevokedDate != null;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive => !IsRevoked && !IsExpired;
}
