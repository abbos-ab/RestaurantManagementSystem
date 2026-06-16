namespace Restaurant.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Email { get; set; }

    public required PhoneNumber PhoneNumber { get; set; }

    public required string Password { get; set; }

    public bool IsActive { get; set; }


    public ICollection<Group> Groups { get; set; }
        = new List<Group>();
}