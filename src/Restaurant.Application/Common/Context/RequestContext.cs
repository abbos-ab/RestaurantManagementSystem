using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Restaurant.Application.Common.Context;

public sealed class RequestContext : IRequestContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public long? UserId
    {
        get
        {
            var userId = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return long.TryParse(userId, out var id) ? id : null;
        }
    }


    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();


    public string? UserAgent => _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].FirstOrDefault();
}