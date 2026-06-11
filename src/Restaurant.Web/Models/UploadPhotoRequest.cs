namespace Restaurant.Web.Models;

public sealed class UploadPhotoRequest
{
    /// <summary>
    /// Сортировка по позиции
    /// </summary>
    public required int Order { get; init; }

    /// <summary>
    /// Ширина изображения
    /// </summary>
    public required double FileWidth { get; init; }

    /// <summary>
    /// Высота изображения
    /// </summary>
    public required double FileHeight { get; init; }

    /// <summary>
    /// Фото
    /// </summary>
    public required IFormFile Photo { get; init; }
}