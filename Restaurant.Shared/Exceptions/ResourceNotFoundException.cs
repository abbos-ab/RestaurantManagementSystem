using Restaurant.Shared.Common.Models;

namespace Restaurant.Shared.Exceptions;

/// <summary>
/// Запрашиваемый ресурс не найден.
/// </summary>
public class ResourceNotFoundException : BusinessLogicException
{
    public ResourceNotFoundException(Error error)
        : base(error) { }

    public ResourceNotFoundException(Error error, Exception innerException)
        : base(error, innerException) { }
}