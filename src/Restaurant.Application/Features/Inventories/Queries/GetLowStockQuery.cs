using Ardalis.Specification;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Inventories.Queries;

public record GetLowStockQuery : IQuery<IEnumerable<InventoryDto>>;

public class GetLowStockQueryHandler : IQueryHandler<GetLowStockQuery, IEnumerable<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly InventoryMapper _inventoryMapper;

    public GetLowStockQueryHandler(IInventoryRepository inventoryRepository, InventoryMapper inventoryMapper)
    {
        _inventoryRepository = inventoryRepository;
        _inventoryMapper = inventoryMapper;
    }

    public async Task<IEnumerable<InventoryDto>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
    {
        var spec = new DbSpecification<Inventory>();
        spec.Query.Where(x => x.Quantity > 10);

        var inventories = await _inventoryRepository.ListAsync(spec, cancellationToken);
        if (!inventories.Any())
            return [];

        return _inventoryMapper.Map(inventories);
    }
}