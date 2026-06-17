namespace Restaurant.Mediator.Helper.Common.Models;

/// <summary>
/// Модель представляющая результат запроса файла.
/// </summary>
/// <param name="Stream"><see cref="System.IO.Stream"/> содержащий файл.</param>
/// <param name="FileName">Название файла.</param>
/// <param name="ContentType">MIME тип файла.</param>
public sealed record FileResponse(Stream Stream, string FileName, string? ContentType = null);