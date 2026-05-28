using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Authentications.Specifications;

public sealed record RefreshTokenByTokenParams
{
    public required string Token { get; init; }

    public long? UserId { get; init; }

    public bool IncludeUser { get; init; }

    public bool AsNoTracking { get; init; }
}

public sealed class RefreshTokenByTokenSpec : Specification<RefreshToken>
{
    public RefreshTokenByTokenParams Params { get; }

    public RefreshTokenByTokenSpec(RefreshTokenByTokenParams @params)
    {
        Params = @params;

        if (@params.AsNoTracking)
            Query.AsNoTracking();

        if (@params.IncludeUser)
            Query.Include(x => x.User);

        if (@params.UserId.HasValue)
            Query.Where(x => x.UserId == @params.UserId.Value);

        Query.Where(x => x.Token == @params.Token);
    }
}