using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Waiters.Commands;

namespace Restaurant.Web.Controllers;

[ApiController]
[Route("/api[controller]/[action]")]
public class WaiterController : Controller
{
    private readonly IMediator _mediator;

    public WaiterController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{orderId:long}/take")]
    public async Task<OrderDto> TakeOrder(
        long orderId,
        long waiterId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new TakeOrderCommand(orderId, waiterId), cancellationToken);
    }

    [HttpPost("{orderId:long}/reject")]
    public async Task<bool> RejectOrder(
        long orderId,
        long waiterId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new RejectOrderCommand(orderId, waiterId), cancellationToken);
    }
}