using Auth.Application.UsersGroups.Specifications;
using Restaurant.Application.Features.UsersGroups.Models;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;

namespace Restaurant.Application.Features.UsersGroups.Queries;

public sealed record GetGroupsByUserId(long UserId) : IQuery<IReadOnlyList<GroupDto>>;

internal sealed class GetGroupsByUserIdHandler : IQueryHandler<GetGroupsByUserId, IReadOnlyList<GroupDto>>
{
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly UserGroupMapper _mapper;

    public GetGroupsByUserIdHandler(IUserGroupRepository userGroupRepository, UserGroupMapper mapper)
    {
        _userGroupRepository = userGroupRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<GroupDto>> Handle(GetGroupsByUserId request, CancellationToken cancellationToken)
    {
        var @params = new UserGroupByUserIdParams
        {
            UserId = request.UserId,
            AsNoTracking = true,
            IncludeGroup = true,
        };

        var relations = await _userGroupRepository.ListAsync(
            new UserGroupByUserIdSpec(@params),
            cancellationToken
        );

        return _mapper.Map(relations.Select(x => x.Group));
    }
}
