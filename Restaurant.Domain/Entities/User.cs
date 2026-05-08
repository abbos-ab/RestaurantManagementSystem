namespace Restaurant.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string PasswordHash { get; set; } = string.Empty;
}

public enum UserRole
{
    Admin,
    Waiter,
    Chef
}

