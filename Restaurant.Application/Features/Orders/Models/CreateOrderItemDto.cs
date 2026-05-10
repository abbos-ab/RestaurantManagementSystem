using FluentValidation;

namespace Restaurant.Application.Features.Orders.Models;

public class CreateOrderItemDto
{
    public long DishId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("DishId must be greater than 0.");
        
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Quantity must be greater than or equal 0.");
    }
}