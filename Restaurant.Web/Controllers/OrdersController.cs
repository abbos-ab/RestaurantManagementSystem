using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Orders.Commands;
using Restaurant.Application.Features.Orders.Models;
using Restaurant.Application.Features.Orders.Queries;
using Restaurant.Domain.Entities;
using Restaurant.Shared.Common.Models;

namespace Restaurant.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<PaginatedResult<OrderDto>> GetAll(
        [FromQuery] PaginationInfo pagination,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new GetAllOrders(pagination),
            cancellationToken);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrderDto>> GetById(
        long id,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetOrderById(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CreateOrderItem(
        [FromQuery]long orderId,
        [FromBody] List<CreateOrderItemDto> items,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new CreateOrderItemsCommand(orderId, items), cancellationToken);
    }

    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(
        long id,
        [FromQuery] OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateOrderStatusCommand(id, status),
            cancellationToken);

        return NoContent();
    }

    [HttpPut("items/{itemId:long}/status")]
    public async Task<IActionResult> UpdateItemStatus(
        long itemId,
        [FromQuery] OrderItemStatus status,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateOrderItemStatusCommand(itemId, status),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(
        long id,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new DeleteOrderCommand(id),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("by-status")]
    public async Task<List<OrderDto>> GetByStatus(
        [FromQuery] OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new GetOrdersByStatusQuery(status),
            cancellationToken);
    }
}