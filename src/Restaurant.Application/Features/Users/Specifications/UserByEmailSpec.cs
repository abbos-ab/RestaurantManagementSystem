using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Users.Specifications;

public class UserByEmailSpec : Specification<User>
{
    public string Email { get; set; }

    public UserByEmailSpec(string email ,bool asNoTracking = true)
    {
        Email = email;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(u => u.Email == email);
    }
}