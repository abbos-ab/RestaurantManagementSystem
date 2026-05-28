using FluentValidation;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Users.Commands;

public sealed record UpdateUserCommand(
    long Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    UserRole Role
) : ICommand<UserDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();
    }
}

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _userMapper;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        UserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            throw new BusinessLogicException(UserErrors.NotFound);

        var newPhone = PhoneNumber.Create(request.PhoneNumber);
        
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = newPhone;
        user.Role = request.Role;

        await _userRepository.UpdateAsync(user, cancellationToken);

        return _userMapper.Map(user);
    }
}