using System.Security.Cryptography;
using System.Text;
using FluentValidation;
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

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        UserMapper userMapper,
        TimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
        _timeProvider = timeProvider;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var spec = new UserByEmailOrPhoneSpec(request.Email, request.PhoneNumber);
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
            Password = Hash(request.Password),
            IsActive = true,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return _userMapper.Map(user);
    }
    
    private string Hash(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}