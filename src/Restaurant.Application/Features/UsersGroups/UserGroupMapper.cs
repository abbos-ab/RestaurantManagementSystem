using Restaurant.Application.Features.UsersGroups.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.UsersGroups;

[Mapper]
public partial class UserGroupMapper
{
    public partial GroupDto Map(Group group);

    public partial IReadOnlyList<GroupDto> Map(IEnumerable<Group> groups);
}
