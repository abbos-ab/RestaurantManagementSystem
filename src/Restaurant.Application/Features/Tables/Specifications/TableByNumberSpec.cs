using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Tables.Specifications;

public class TableByNumberSpec : Specification<Table>
{
    public int Number { get; set; }

    public TableByNumberSpec(int number, bool asNoTracking = false)
    {
        Number = number;
        
        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.Number == number);
    }
}