using FluentValidation;

namespace Restaurant.Application.Features.Orders.Models;

public class CreateOrderItemDto
{
    public long DishId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemCommandValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("DishId must be greater than 0.");
        
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be greater than or equal 0.");
    }
}