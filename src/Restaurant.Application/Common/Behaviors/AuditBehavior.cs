using System.Text.Json;
using MediatR;
using Restaurant.Application.Common.Context;
using Restaurant.Application.Features.AuditLogs.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Extensions;

namespace Restaurant.Application.Common.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IBaseCommand
{
    private readonly IAuditRepository _auditRepository;
    private readonly IRequestContext _requestContext;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public AuditBehavior(
        IAuditRepository auditRepository,
        IRequestContext requestContext,
        ICurrentUserAccessor currentUserAccessor)
    {
        _auditRepository = auditRepository;
        _requestContext = requestContext;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IAnonymousCommand)
            return await next(cancellationToken);

        var audit = new AuditLog
        {
            UserId = _currentUserAccessor.GetRequiredUser().GetUserId(),
            Action = typeof(TRequest).Name,
            EntityName = GetEntityName(typeof(TRequest).Name),
            IpAddress = _requestContext.IpAddress,
            UserAgent = _requestContext.UserAgent,
            NewValues = JsonSerializer.Serialize(request),
            CreatedAt = DateTime.UtcNow,
            IsSuccess = false
        };
        
        try
        {
            var response = await next(cancellationToken);
            audit.IsSuccess = true;
            
            await _auditRepository.AddAsync(audit, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            audit.IsSuccess = false;
            audit.ErrorMessage = ex.Message;
            
            await _auditRepository.AddAsync(audit, cancellationToken);

            throw;
        }
    }


    private static string GetEntityName(string commandName)
    {
        return commandName
            .Replace("Create", "")
            .Replace("Update", "")
            .Replace("Delete", "")
            .Replace("Command", "");
    }
}