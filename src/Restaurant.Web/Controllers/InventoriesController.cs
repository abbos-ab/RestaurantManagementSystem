using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Inventories.Commands;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Queries;
using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class InventoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<PaginatedResult<InventoryDto>> GetAll(
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAllInventories(paginationInfo), cancellationToken);
    }

    [HttpGet("{inventoryId:long}")]
    public async Task<InventoryDto?> GetById(
        long inventoryId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetInventoryById(inventoryId), cancellationToken);
    }

    [HttpPost]
    public async Task<InventoryDto> Create(
        CreateInventoryCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPut("{inventoryId:long}")]
    public async Task<InventoryDto> Update(
        long inventoryId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new UpdateInventoryCommand(inventoryId, quantity),
            cancellationToken);
    }

    [HttpDelete("{inventoryId:long}")]
    public async Task<IActionResult> Delete(
        long inventoryId, 
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteInventoryCommand(inventoryId), cancellationToken);
        return NoContent();
    }
}