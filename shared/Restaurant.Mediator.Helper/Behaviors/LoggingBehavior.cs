using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurant.Mediator.Helper.Common.Extensions;

namespace Oshiqona.Mediator.Helper.Behaviors;

/// <summary>
/// Промежуточный обработчик пайплайна для логгирования запросов через MediatR.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_logger.IsEnabled(LogLevel.Information))
            return await next(cancellationToken);

        var requestName = typeof(TRequest).GetDisplayName();

        _logger.LogInformation("Вызван запрос для тип {RequestType}.", requestName);

        var sw = Stopwatch.StartNew();

        try
        {
            return await next(cancellationToken);
        }
        finally
        {
            sw.Stop();

            _logger.LogInformation(
                "Завершен вызов запроса для типа {RequestType} за {ElapsedMilliseconds} мс.",
                requestName,
                sw.Elapsed.TotalMilliseconds.ToString("N0")
            );
        }
    }
}
