using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Waiters.Commands;

namespace Restaurant.Web.Controllers;

public class WaiterController : BaseController
{
    public WaiterController(IMediator mediator) : base(mediator)
    {
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

    [HttpPost("{tableId:long}/CalledWaiter")]
    public async Task<bool> CalledWaiter(
        long tableId, 
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new CallWaiterCommand(tableId), cancellationToken);
    }
}