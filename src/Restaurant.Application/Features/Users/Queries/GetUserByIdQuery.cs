using FluentValidation;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(long UserId) : IQuery<UserDto>;

// ReSharper disable once UnusedType.Global
public sealed class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}

internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _userMapper;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        UserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new BusinessLogicException(UserErrors.NotFound);

        return _userMapper.Map(user);
    }
}