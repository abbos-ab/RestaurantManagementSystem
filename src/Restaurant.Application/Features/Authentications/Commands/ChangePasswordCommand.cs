using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.Users.Specifications;
using Restaurant.Application.Features.Users.Validators;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Authentications.Commands;

public sealed record ChangePasswordCommand(
    string PhoneNumber,
    string Token,
    string NewPassword
) : ICommand;

// ReSharper disable once UnusedType.Global
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).SetValidator(new PhoneValidator());

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Необходимо указать токен.");
    }
}

internal sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserTokenProvider _userTokenProvider;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IUserTokenProvider userTokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userTokenProvider = userTokenProvider;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
        
        var user = await _userRepository.FirstOrDefaultAsync(
            new UserByPhoneSpec(phoneNumber),
            cancellationToken
        );

        if (user is null)
            throw new UnauthorizedException(AuthErrors.Unauthorized);

        if (!_userTokenProvider.ValidateToken(user, request.Token))
            throw new UnauthorizedException(AuthErrors.Unauthorized);

        var newPasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        if (user.Password == newPasswordHash)
            throw new BusinessLogicException(AuthErrors.PasswordShouldBeNew);

        user.Password = newPasswordHash;
        user.IsActive = true;

        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
