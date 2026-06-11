using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Inventories.Commands;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Queries;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.Groups;

namespace Restaurant.Web.Controllers;

public class InventoriesController : BaseController
{
    public InventoriesController(IMediator mediator) : base(mediator)
    {
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
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<InventoryDto> Create(
        CreateInventoryCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPut("{inventoryId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
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
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<IActionResult> Delete(
        long inventoryId, 
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteInventoryCommand(inventoryId), cancellationToken);
        return NoContent();
    }
}