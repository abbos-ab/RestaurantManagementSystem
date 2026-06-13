using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Users.Models;

public sealed class UserDto
{
    public long Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public PhoneNumber PhoneNumber { get; set; }

    public ICollection<GroupDto> Groups { get; set; }
        = new List<GroupDto>();

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}