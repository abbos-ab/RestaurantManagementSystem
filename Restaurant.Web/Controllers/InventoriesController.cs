using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Inventories.Commands;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Queries;
using Restaurant.Shared.Common.Models;

namespace Restaurant.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetAllInventories(paginationInfo), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<InventoryDto> GetById(long id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetInventoryById(id), cancellationToken);
    }

    [HttpPost]
    public async Task<InventoryDto> Create(CreateInventoryCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<InventoryDto> Update(long id, int quantity, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new UpdateInventoryCommand(id, quantity), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteInventoryCommand(id), cancellationToken);
        return NoContent();
    }
}