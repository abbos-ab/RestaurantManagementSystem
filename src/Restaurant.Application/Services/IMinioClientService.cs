using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Services;

public interface IMinioClientService
{
    Task<string> PresignedGetObjectAsync(string objectName, TimeSpan expiry);

    Task<FileResponse> GetStreamObjectAsync(string objectName);

    Task SaveObjectAsync(string objectName, Stream stream, string? contentType = null);

    Task RemoveObjectAsync(string objectName);
}