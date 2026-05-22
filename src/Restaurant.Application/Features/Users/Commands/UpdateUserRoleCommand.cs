using FluentValidation;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Users.Commands;

public sealed record UpdateUserRoleCommand(long UserId, UserRole Role) : ICommand<UserDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be greater than 0");

        RuleFor(x => x.Role)
            .IsInEnum();
    }
}

internal sealed class UpdateUserRoleCommandHadler : ICommandHandler<UpdateUserRoleCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _userMapper;

    public UpdateUserRoleCommandHadler(IUserRepository userRepository, UserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<UserDto> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new BusinessLogicException(UserErrors.NotFound);

        user.Role = request.Role;
        await _userRepository.UpdateAsync(user, cancellationToken);
        
        return _userMapper.Map(user);
    }
}