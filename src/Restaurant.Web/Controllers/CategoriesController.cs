using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Categories.Commands;
using Restaurant.Application.Features.Categories.Models;
using Restaurant.Application.Features.Categories.Queries;
using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Web.Controllers;

public class CategoriesController : BaseController
{
    public CategoriesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<PaginatedResult<CategoryDto>> GetAll(
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new GetAllCategories(paginationInfo), cancellationToken);
    }

    [HttpGet("{categoryId:long}")]
    public async Task<CategoryDto?> GetById(
        long categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new GetCategoryById(categoryId),
            cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{categoryId:long}")]
    public async Task<NoContentResult> Update(
        long categoryId,
        [FromBody] CreateCategoryCommand request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new UpdateCategoryCommand(
                categoryId,
                request.Name,
                request.Description
            ),
            cancellationToken);

        return new NoContentResult();
    }

    [HttpDelete("{categoryId:long}")]
    public async Task<NoContentResult> Delete(
        long categoryId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(
            new DeleteCategoryCommand(categoryId),
            cancellationToken);

        return new NoContentResult();
    }

    [HttpGet]
    public async Task<List<CategoryDto>> GetByName(
        [FromQuery] string name,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(
            new GetCategoryByName(name),
            cancellationToken);
    }
}