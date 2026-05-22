namespace Restaurant.Application.Features.UsersGroups.Models;

public sealed class GroupDto
{
    public long Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }
}
