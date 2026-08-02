using Microsoft.AspNetCore.Http;

namespace Restaurant.Application.Common.Interfaces;

public interface IExceptionNotifier
{
    Task NotifyAsync(Exception exception, HttpContext context, CancellationToken cancellationToken = default);
}