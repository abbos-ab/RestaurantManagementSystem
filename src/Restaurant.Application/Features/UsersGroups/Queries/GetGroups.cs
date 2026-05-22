using Auth.Domain.Models;
using Restaurant.Application.Features.UsersGroups.Models;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.UsersGroups.Queries;

public sealed record GetGroups : IQuery<IReadOnlyList<GroupDto>>;

internal sealed class GetGroupsHandler : IQueryHandler<GetGroups, IReadOnlyList<GroupDto>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly UserGroupMapper _mapper;

    public GetGroupsHandler(IGroupRepository groupRepository, UserGroupMapper mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<GroupDto>> Handle(GetGroups request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Group>();
        var groups = await _groupRepository.ListAsync(spec, cancellationToken);

        return _mapper.Map(groups);
    }
}
