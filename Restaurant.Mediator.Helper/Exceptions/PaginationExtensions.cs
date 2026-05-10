using Ardalis.Specification;
using Restaurant.Shared.Common.Models;

namespace Restaurant.Shared.Exceptions;

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