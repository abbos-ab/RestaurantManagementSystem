using FluentValidation;

namespace Restaurant.Application.Features.Users.Validators;

public sealed class PhoneValidator : AbstractValidator<string>
{
    public PhoneValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("Поле Телефон необходимо заполнить")
            .MinimumLength(12)
            .WithMessage("Минимальная длина строки 12 символов")
            .MaximumLength(13)
            .WithMessage("Максимальная длина строки 13 символов");
    }
}