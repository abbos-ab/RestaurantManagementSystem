using Ardalis.Specification;
using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Mediator.Helper.Common.Extensions;

public static class PaginationExtensions
{
    public static ISpecificationBuilder<T> WithPagination<T>(
        this ISpecificationBuilder<T> builder,
        PaginationInfo paginationInfo)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(paginationInfo);

        return builder
            .Skip(paginationInfo.Size * paginationInfo.Index)
            .Take(paginationInfo.Size);
    }
}