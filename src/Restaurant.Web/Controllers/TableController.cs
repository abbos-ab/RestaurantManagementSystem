using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Tables.Commands;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Queries;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.Groups;

namespace Restaurant.Web.Controllers;

public class TablesController : BaseController
{
    public TablesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<ActionResult<TableDto>> Create(
        CreateTableCommand command,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpPut("{tableId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<ActionResult<TableDto>> Update(
        long tableId,
        int number,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new UpdateTableNumberCommand(tableId, number), cancellationToken));
    }

    [HttpPut("{tableId:long}")]
    public async Task<ActionResult<TableDto>> UpdateStatus(
        long tableId,
        TableStatus status,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new UpdateTableStatusCommand(tableId, status), cancellationToken));
    }

    [HttpPut("{tableId:long}/capacity")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs, GroupNames.Waiters)]
    public async Task<ActionResult<TableDto>> UpdateCapacity(
        long tableId,
        int capacity,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new UpdateTableCapacityCommand(tableId, capacity), cancellationToken));
    }

    [HttpDelete("{tableId:long}")]
    [GroupAuthorize(GroupNames.Administrators, GroupNames.Chefs)]
    public async Task<IActionResult> Delete(
        long tableId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteTableCommand(tableId), cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<TableDto>>> GetAll(
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetAllTablesQuery(paginationInfo), cancellationToken));
    }

    [HttpGet("{tableId:long}")]
    public async Task<ActionResult<TableDto>> GetById(
        long tableId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetTableByIdQuery(tableId), cancellationToken));
    }

    [HttpGet("{capacity:int}")]
    public async Task<PaginatedResult<TableDto>> GetByCapacity(
        int capacity,
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetTablesByCapacity(capacity, paginationInfo), cancellationToken);
    }
}