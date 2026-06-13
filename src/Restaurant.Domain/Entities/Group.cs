using System.ComponentModel.DataAnnotations;

namespace Restaurant.Domain.Entities;

public sealed class Group : BaseEntity
{
    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(500)]
    public required string Description { get; set; }

    public ICollection<User> Users { get; set; }
        = new List<User>();
}
