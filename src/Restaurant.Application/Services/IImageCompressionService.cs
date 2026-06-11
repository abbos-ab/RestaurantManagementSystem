namespace Restaurant.Application.Services;

public interface IImageCompressionService
{
    Task<(Stream Stream, string ContentType, long Size)> CompressAsync(
        Stream input,
        string contentType,
        CancellationToken cancellationToken = default
    );
}