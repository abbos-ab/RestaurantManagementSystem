using FluentValidation;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.Authentications.Commands;

public sealed record AuthenticateCommand(
    string PhoneNumber,
    string Password
) : ICommand;

// ReSharper disable once UnusedType.Global
public sealed class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
{
    public AuthenticateCommandValidator()
    {
    }
}