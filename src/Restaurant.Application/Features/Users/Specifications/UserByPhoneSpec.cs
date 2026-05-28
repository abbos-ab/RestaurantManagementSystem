using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Users.Specifications;

public sealed class UserByPhoneSpec : Specification<User>
{
    public PhoneNumber Phone { get; }

    public UserByPhoneSpec(PhoneNumber phone, bool asNoTracking = false)
    {
        Phone = phone;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(u => u.PhoneNumber == phone);
    }
}