using FluentValidation;

namespace Restaurant.Application.Features.Carts.Models;

public class CreateCartItemDto
{
    public long DishId { get; set; }
    public int Quantity { get; set; }
}

public class CreateCartItemDtoValidator : AbstractValidator<CreateCartItemDto>
{
    public CreateCartItemDtoValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("Dish id must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Quantity must be greater than or equal 1");
    }
}