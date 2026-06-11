using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Restaurant.Infrastructure.Settings;

namespace Restaurant.Web.Services;

internal sealed class MinioBucketBackgroundService : IHostedService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _minioSettings;

    public MinioBucketBackgroundService(IMinioClient minioClient, IOptions<MinioSettings> options)
    {
        _minioClient = minioClient;
        _minioSettings = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var args = new BucketExistsArgs().WithBucket(_minioSettings.BucketName);
        var bucketExist = await _minioClient.BucketExistsAsync(args, cancellationToken);

        if (bucketExist)
            return;

        var mbArgs = new MakeBucketArgs().WithBucket(_minioSettings.BucketName);
        await _minioClient.MakeBucketAsync(mbArgs, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}