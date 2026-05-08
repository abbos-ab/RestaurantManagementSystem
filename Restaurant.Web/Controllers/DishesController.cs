using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Dishes.Commands;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Queries;
using Restaurant.Shared.Common.Models;

[ApiController]
[Route("api/[controller]")]
public class DishesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DishesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<PaginatedResult<DishDto>> GetAll(
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAllDishes(paginationInfo), cancellationToken);
    }

    [HttpGet("{dishId}")]
    public async Task<DishDto?> GetById(long dishId, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetDishByIdQuery(dishId), cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDishCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{dishId}")]
    public async Task<NoContentResult> Update(
        long dishId,
        [FromBody] CreateDishCommand request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateDishCommand(
                dishId,
                request.Name,
                request.CategoryId,
                request.Price,
                request.Description,
                request.IsActive
            ),
            cancellationToken
        );
        return new NoContentResult();
    }

    [HttpDelete("{dishId}")]
    public async Task<NoContentResult> Delete(long dishId)
    {
        await _mediator.Send(new DeleteDishCommand(dishId));
        return new NoContentResult();
    }
}