using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Tables.Specifications;

public class TableByCapacitySpec : Specification<Table>
{
    public int Capacity { get; set; }

    public TableByCapacitySpec(int  capacity, bool asNoTracking = true)
    {
        Capacity = capacity;
        
        if (asNoTracking)
            Query.AsNoTracking();
        
        Query.Where(x => x.Capacity == capacity);
    }
}