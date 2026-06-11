using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Medias;

public static class MediaErrors
{
    public static readonly Error NotFound = new(
        "Media.NotFound",
        "Media not found"
    );
}