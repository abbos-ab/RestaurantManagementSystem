using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Categories.Specifications;

public class CategoryByNameSpec : Specification<Category>
{
    public string CategoryName { get; set; }

    public CategoryByNameSpec(string categoryName, bool asNoTracking = false)
    {
        CategoryName = categoryName;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.Name == categoryName);
    }
}