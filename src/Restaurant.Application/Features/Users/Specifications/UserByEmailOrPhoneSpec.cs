using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Users.Specifications;

public class UserByEmailOrPhoneSpec : Specification<User>
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public UserByEmailOrPhoneSpec(string email,string phoneNumber ,bool asNoTracking = true)
    {
        Email = email;
        PhoneNumber = phoneNumber;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(u => u.Email == email || u.PhoneNumber.ToString() == phoneNumber);
    }
}