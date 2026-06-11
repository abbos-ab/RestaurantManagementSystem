using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Dishes.Commands;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Queries;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.Groups;

namespace Restaurant.Web.Controllers;

public class DishesController : BaseController
{
    public DishesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<PaginatedResult<DishDto>> GetAll(
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new GetAllDishes(paginationInfo),
            cancellationToken);
    }

    [HttpGet("{dishId:long}")]
    public async Task<DishDto?> GetById(
        long dishId, 
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new GetDishById(dishId),
            cancellationToken);
    }

    [HttpPost]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<IActionResult> Create(CreateDishCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{dishId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
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

    [HttpPut("{dishId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<ActionResult<DishDto>> UpdatePrice(
        long dishId,
        decimal price,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new UpdateDishPriceCommand(dishId, price), cancellationToken));
    }
    
    [HttpDelete("{dishId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<NoContentResult> Delete(long dishId)
    {
        await _mediator.Send(new DeleteDishCommand(dishId));
        return new NoContentResult();
    }
}