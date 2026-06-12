using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.Users.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    UserRole Role,
    string Password
) : ICommand<UserDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.Password)
            .MinimumLength(6);
    }
}

internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly UserMapper _userMapper;
    private readonly TimeProvider _timeProvider;
    private readonly IPasswordHasher<User> _passwordHasher;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        UserMapper userMapper,
        TimeProvider timeProvider, 
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
        _timeProvider = timeProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserByEmailSpec(request.Email);
        var exists = await _userRepository.AnyAsync(spec,cancellationToken);
        if (exists)
            throw new BusinessLogicException(UserErrors.AlreadyExists);

        var phone = PhoneNumber.Create(request.PhoneNumber);
        
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = phone,
            Role = request.Role,
            Password = "",
            IsActive = true,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        };
        
        user.Password = _passwordHasher.HashPassword(user, request.Password);
        
        await _userRepository.AddAsync(user, cancellationToken);

        return _userMapper.Map(user);
    }
}