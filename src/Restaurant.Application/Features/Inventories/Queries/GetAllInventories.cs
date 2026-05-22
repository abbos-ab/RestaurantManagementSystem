using Ardalis.Specification;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Inventories.Queries;

public sealed record GetAllInventories(PaginationInfo PaginationInfo) : IQuery<PaginatedResult<InventoryDto>>;

internal sealed class GetAllInventoriesHandler : IQueryHandler<GetAllInventories, PaginatedResult<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly InventoryMapper _mapper;

    public GetAllInventoriesHandler(IInventoryRepository inventoryRepository, InventoryMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<InventoryDto>> Handle(GetAllInventories request,
        CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Inventory>();

        spec.Query
            .Include(x => x.Dish)
            .WithPagination(request.PaginationInfo)
            .OrderBy(x => x.Id);

        var inventories = await _inventoryRepository.ListAsync(spec, cancellationToken);
        var totalCound = await _inventoryRepository.CountAsync(spec, cancellationToken);

        var mapperInventory = _mapper.Map(inventories);

        return new PaginatedResult<InventoryDto>(mapperInventory, totalCound);
    }
}