using Auth.Domain.Models;

namespace Restaurant.Domain.Entities;

public sealed class UserGroupRelation
{
    public required long UserId { get; set; }
    public User User { get; set; } = null!;

    public required long GroupId { get; set; }
    public Group Group { get; set; } = null!;
}
