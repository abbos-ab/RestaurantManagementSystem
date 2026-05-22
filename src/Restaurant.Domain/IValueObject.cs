namespace Restaurant.Domain;

/// <summary>
/// Интерфейс представляющий объект значения.
/// </summary>
/// <typeparam name="TObject">Тип объекта значения.</typeparam>
/// <typeparam name="TValue">Тип хранимого в объекте значения.</typeparam>
public interface IValueObject<TObject, TValue>
    : IComparable,
        IComparable<TObject>,
        IEquatable<TObject>
    where TObject : IValueObject<TObject, TValue>
{
    /// <summary>
    /// Текущее значение.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Создает экземпляр объекта <typeparamref name="TObject"/>.
    /// </summary>
    /// <param name="value">Значение объекта.</param>
    /// <returns>Новый экземпляр <typeparamref name="TObject"/>.</returns>
    public static abstract TObject FromValue(TValue value);
}