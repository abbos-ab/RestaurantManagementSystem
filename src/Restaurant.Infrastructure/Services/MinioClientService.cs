using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Restaurant.Application.Services;
using Restaurant.Infrastructure.Settings;
using Restaurant.Mediator.Helper.Common;
using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Infrastructure.Services;

public sealed class MinioClientService : IMinioClientService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;

    public MinioClientService(IMinioClient minioClient, IOptions<MinioSettings> options)
    {
        _minioClient = minioClient;
        _settings = options.Value;
    }

    public async Task<string> PresignedGetObjectAsync(string objectName, TimeSpan expiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectName);

        var args = new PresignedGetObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName)
            .WithExpiry((int)expiry.TotalSeconds);

        return await _minioClient.PresignedGetObjectAsync(args);
    }

    public async Task<FileResponse> GetStreamObjectAsync(string objectName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectName);

        var stream = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName)
            .WithCallbackStream((s, c) => s.CopyToAsync(stream, c));

        var res = await _minioClient.GetObjectAsync(args);

        stream.Position = 0;

        return new FileResponse(stream, objectName, res.ContentType);
    }

    public async Task SaveObjectAsync(string objectName, Stream stream, string? contentType = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectName);

        var args = new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType ?? ContentTypes.OctetStream);

        await _minioClient.PutObjectAsync(args);
    }

    public async Task RemoveObjectAsync(string objectName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objectName);

        var args = new RemoveObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(args);
    }
}
