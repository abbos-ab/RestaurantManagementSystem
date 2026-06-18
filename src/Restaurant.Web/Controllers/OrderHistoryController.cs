using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.OrderHistories.Commands;
using Restaurant.Application.Features.OrderHistories.Models;
using Restaurant.Application.Features.OrderHistories.Queries;
using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Web.Controllers;

public class OrderHistoriesController : BaseController
{
    public OrderHistoriesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPut]
    public async Task<ActionResult<OrderHistoryDto>> Update(
        UpdateOrderHistoryCommand command,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpDelete("{historyId:long}")]
    public async Task<IActionResult> Delete(
        long historyId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteOrderHistoryCommand(historyId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{historyId:long}")]
    public async Task<ActionResult<OrderHistoryDto>> GetById(
        long historyId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetOrderHistoryById(historyId), cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<OrderHistoryDto>>> GetAll(
        PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetAllOrderHistories(paginationInfo), cancellationToken));
    }
}