namespace Restaurant.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public UserRole Role { get; set; }

    public string PasswordHash { get; set; }

    public bool IsActive { get; set; }
}

public enum UserRole
{
    Admin,
    Waiter,
    Chef
}

