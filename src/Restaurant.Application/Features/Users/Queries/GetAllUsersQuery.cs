using Ardalis.Specification;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Users.Queries;

public sealed record GetAllUsersQuery(PaginationInfo PaginationInfo) : IQuery<PaginatedResult<UserDto>>;

internal sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, PaginatedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _userMapper;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        UserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<PaginatedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<User>();
        spec.Query
            .WithPagination(request.PaginationInfo)
            .OrderBy(x => x.FirstName);

        var users = await _userRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _userRepository.CountAsync(spec, cancellationToken);
        
        var mapperUsers = _userMapper.Map(users);
        
        return new PaginatedResult<UserDto>(mapperUsers, totalCount);
    }
}