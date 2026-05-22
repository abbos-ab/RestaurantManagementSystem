using Restaurant.Domain;
using System.ComponentModel.DataAnnotations;

namespace Auth.Domain.Models;

public sealed class Group : BaseEntity
{
    [StringLength(100)]
    public required string Name { get; set; }

    [StringLength(500)]
    public required string Description { get; set; }
}
