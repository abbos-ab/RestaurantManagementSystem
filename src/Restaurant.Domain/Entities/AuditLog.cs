namespace Restaurant.Domain.Entities;

public sealed class AuditLog : BaseEntity
{
    public long? UserId { get; set; }
    public User? User { get; set; }

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public long? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }
}