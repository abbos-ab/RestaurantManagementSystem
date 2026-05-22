using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Carts.Commands;
using Restaurant.Application.Features.Carts.Models;

namespace Restaurant.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<CartDto> CreateCart(
        CreateCartCommand request,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new CreateCartCommand(request.TableId, request.Items),
            cancellationToken);
    }

    [HttpPost]
    public async Task<bool> CreateCartItem(
        [FromQuery] long caerId,
        [FromBody] List<CreateCartItemDto> items,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new CreateCartItemCommand(caerId, items),
            cancellationToken);
    }

    [HttpPut]
    public async Task<CartItemDto> UpdateCartItem(
        [FromQuery] long cartId,
        [FromBody] CreateCartItemDto item,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new UpdateCartItemCommand(cartId, item),
            cancellationToken);
    }
}