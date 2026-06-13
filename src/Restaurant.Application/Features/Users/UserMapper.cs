using Restaurant.Application.Features.Users.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Users;

[Mapper]
public partial class UserMapper
{
    public partial UserDto Map(User entity);

    public partial List<UserDto> Map(List<User> entities);

    public partial GroupDto Map(Group entity);
}