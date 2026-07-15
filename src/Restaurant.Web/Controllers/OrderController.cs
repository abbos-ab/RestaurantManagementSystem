using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Orders.Commands;
using Restaurant.Application.Features.Orders.Models;
using Restaurant.Application.Features.Orders.Queries;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Web.Controllers;

public class OrderController : BaseController
{
    public OrderController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<PaginatedResult<OrderDto>> GetAll(
        [FromQuery] PaginationInfo pagination,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAllOrders(pagination), cancellationToken);
    }

    [HttpGet("{orderId:long}")]
    public async Task<ActionResult<OrderDto>> GetById(
        long orderId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetOrderById(orderId), cancellationToken);

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
        [FromQuery] long orderId,
        [FromBody] List<CreateOrderItemDto> items,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new CreateOrderItemsCommand(orderId, items), cancellationToken);
    }

    [HttpPut]
    public async Task<ActionResult<OrderItemDto>> UpdateOrderItem(
        long itemId,
        [FromBody] UpdateOrderItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{orderId:long}/status")]
    public async Task<IActionResult> UpdateStatus(
        long orderId,
        [FromQuery] OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateOrderStatusCommand(orderId, status), cancellationToken);

        return NoContent();
    }

    [HttpPut("items/{itemId:long}/status")]
    public async Task<IActionResult> UpdateItemStatus(
        long itemId,
        [FromQuery] OrderItemStatus status,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateOrderItemStatusCommand(itemId, status), cancellationToken);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(
        DeleteOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new DeleteOrderCommand(request.TableId, request.OrderId),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("items/")]
    public async Task<IActionResult> Delete(
        long orderId,
        List<long> items,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteOrderItemsCommand(orderId, items), cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<List<OrderDto>> GetByStatus(
        [FromQuery] OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetOrdersByStatus(status), cancellationToken);
    }

    [HttpGet]
    public async Task<IEnumerable<OrderDto>> GetByTableId(
        long tableId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetOrdersByTableQuery(tableId), cancellationToken);
    }
}