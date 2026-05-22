using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Restaurant.Domain.Entities;

/// <summary>
/// Модель представляющее количество позиции.
/// </summary>
[DebuggerDisplay("Value = {Value}")]
public readonly struct PhoneNumber
    : IValueObject<PhoneNumber, long>
{
    // Код страны
    private const long CountryCode = 992;

    // Длина номера без кода страны (9 цифр: 987654321)
    private const int LocalNumberLength = 9;

    // Полная длина номера с кодом страны (12 цифр: 992987654321)
    private const int FullNumberLength = 12;

    private PhoneNumber(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        Value = value;
    }

    /// <inheritdoc />
    public long Value { get; }

    /// <summary>
    /// Создаёт экземпляр из числового значения.
    /// Ожидает полный номер с кодом страны: 992XXXXXXXXX
    /// </summary>
    public static PhoneNumber FromValue(long value)
    {
        Validate(value);
        return new(value);
    }

    /// <summary>
    /// Парсит строку в <see cref="PhoneNumber"/>.
    /// Принимает форматы: +992987654321, 992987654321
    /// </summary>
    /// <exception cref="FormatException">Если строка не является корректным номером телефона.</exception>
    public static PhoneNumber Parse(string value)
    {
        if (!TryParse(value, out var result, out var error))
            throw new FormatException(error);

        return result;
    }

    /// <summary>
    /// Пытается распарсить строку в <see cref="PhoneNumber"/>.
    /// Принимает форматы: +992987654321, 992987654321
    /// </summary>
    public static bool TryParse(string? value, out PhoneNumber result)
        => TryParse(value, out result, out _);

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is null)
            return 1;

        if (obj is not PhoneNumber phoneNumber)
            throw new ArgumentException("Должно быть типа PhoneNumber", nameof(obj));

        return CompareTo(phoneNumber);
    }

    /// <inheritdoc />
    public int CompareTo(PhoneNumber other) => Value.CompareTo(other.Value);

    /// <inheritdoc />
    public bool Equals(PhoneNumber other) => CompareTo(other) == 0;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is PhoneNumber other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc cref="long.ToString()"/>
    public override string ToString() => $"+{Value}";

    /// <inheritdoc cref="decimal.ToString(string?)"/>
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format)
        => Value.ToString(format);

    /// <inheritdoc cref="decimal.ToString(IFormatProvider?)"/>
    public string ToString(IFormatProvider? provider)
        => Value.ToString(provider);

    /// <inheritdoc cref="decimal.ToString(string?, IFormatProvider?)"/>
    public string ToString(
        [StringSyntax(StringSyntaxAttribute.NumericFormat)]
        string? format,
        IFormatProvider? provider)
        => Value.ToString(format, provider);

    private static bool TryParse(string? value, out PhoneNumber result, out string? error)
    {
        result = new(0);
        error = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            error = "Номер телефона не может быть пустым.";
            return false;
        }

        var normalized = value.TrimStart('+');

        if (!long.TryParse(normalized, out var number) || number <= 0)
        {
            error = $"Номер телефона содержит недопустимые символы: '{value}'.";
            return false;
        }

        // Если передали локальный номер без кода страны, добавляем код страны
        var digits = normalized.Length;

        number = digits switch
        {
            LocalNumberLength => CountryCode * (long)Math.Pow(10, LocalNumberLength) + number,
            FullNumberLength => number,
            _ => -1,
        };

        if (number == -1)
        {
            error =
                $"Некорректная длина номера телефона: '{value}'. Ожидается {LocalNumberLength} или {FullNumberLength} цифр.";
            return false;
        }

        if (!IsValidNumber(number, out error))
            return false;

        result = new(number);
        return true;
    }

    private static bool IsValidNumber(long value, out string? error)
    {
        error = null;

        // Проверяем что число имеет ровно FullNumberLength
        var digits = value.ToString().Length;

        if (digits != FullNumberLength)
        {
            error = $"Номер телефона должен содержать {FullNumberLength} цифр, передано: {digits}.";
            return false;
        }

        // Проверяем код страны
        var extractedCode = value / (long)Math.Pow(10, LocalNumberLength);

        if (extractedCode != CountryCode)
        {
            error = $"Неверный код страны: {extractedCode}. Ожидается: {CountryCode}.";
            return false;
        }

        return true;
    }
    
    private static void Validate(long value)
    {
        if (!IsValidNumber(value, out var error))
            throw new ArgumentOutOfRangeException(nameof(value), error);
    }
}