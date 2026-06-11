using Restaurant.Application.Services;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Restaurant.Infrastructure.Services;

internal sealed class ImageCompressionService : IImageCompressionService
{
    private const int MaxSize = 2560;
    private const int WebpQuality = 80;

    public async Task<(Stream Stream, string ContentType, long Size)> CompressAsync(
        Stream input,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (string.Equals(contentType, "image/svg+xml", StringComparison.OrdinalIgnoreCase))
            throw new BusinessLogicException(new Error("Image.SvgNotAllowed", "SVG not allowed"));

        if (!input.CanSeek)
        {
            var copy = new MemoryStream();
            await input.CopyToAsync(copy, cancellationToken);
            copy.Position = 0;
            input = copy;
        }

        input.Position = 0;

        Image image;
        try
        {
            image = await Image.LoadAsync(input, cancellationToken);
        }
        catch
        {
            throw new BusinessLogicException(new Error("Image.Invalid", "Invalid image"));
        }

        image.Metadata.ExifProfile = null;
        image.Metadata.IccProfile = null;

        image.Mutate(x =>
            {
                x.AutoOrient();

                if (image.Width > MaxSize || image.Height > MaxSize)
                {
                    x.Resize(
                        new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(MaxSize, MaxSize),
                        }
                    );
                }
            }
        );

        var output = new MemoryStream();

        var hasAlpha = HasTransparency(image);

        IImageEncoder encoder;

        if (hasAlpha)
        {
            encoder = new WebpEncoder
            {
                Quality = 90,
                FileFormat = WebpFileFormatType.Lossless
            };
        }
        else
        {
            encoder = new WebpEncoder
            {
                Quality = WebpQuality,
            };
        }
        
        await image.SaveAsync(output, encoder, cancellationToken);
        
        output.Position = 0;
        
        return (output, "image/webp", output.Length);
    }

    private static bool HasTransparency(Image image)
    {
        return image.PixelType.BitsPerPixel >= 32;
    }
}