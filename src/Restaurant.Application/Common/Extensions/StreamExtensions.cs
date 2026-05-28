using System.Text;

namespace Restaurant.Application.Common.Extensions;

/// <summary>
/// Utility extensions to streams
/// </summary>
internal static class StreamExtensions
{
    internal static readonly Encoding _defaultEncoding = new UTF8Encoding(false, true);

    public static BinaryReader CreateReader(this Stream stream) => new(stream, _defaultEncoding, true);

    public static BinaryWriter CreateWriter(this Stream stream) => new(stream, _defaultEncoding, true);

    public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader) => new(reader.ReadInt64(), TimeSpan.Zero);

    public static void Write(this BinaryWriter writer, DateTimeOffset value)
    {
        writer.Write(value.UtcTicks);
    }
}