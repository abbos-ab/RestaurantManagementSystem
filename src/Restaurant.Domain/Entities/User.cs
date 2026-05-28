using System.ComponentModel.DataAnnotations;

namespace Restaurant.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Email { get; set; }
    
    public required PhoneNumber PhoneNumber { get; set; }

    public UserRole Role { get; set; }

    public required string Password { get; set; }

    public bool IsActive { get; set; }
}

public enum UserRole
{
    Admin,
    Waiter,
    Chef
}