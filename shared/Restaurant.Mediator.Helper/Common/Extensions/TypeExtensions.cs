using Restaurant.Mediator.Helper.Common.Reflection;
using System.Reflection;

namespace Restaurant.Mediator.Helper.Common.Extensions;

public static class TypeExtensions
{
    private static readonly TypeNameFormatter _typeNameFormatter = new();

    /// <summary>
    /// Возвращает человекочитаемое^_^ название для указанного типа.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetDisplayName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return _typeNameFormatter.GetTypeName(type);
    }

    /// <summary>
    /// Возвращает информацию о конструкторе без параметров.
    /// </summary>
    /// <remarks>
    /// Бросает исключение, если такой конструктор не найден.
    /// </remarks>
    /// <exception cref="MissingMethodException"></exception>
    public static ConstructorInfo GetParameterlessConstructor(this Type type)
    {
        var ctor = type.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            Type.EmptyTypes
        );

        if (ctor is null)
            throw new MissingMethodException($"Parameterless constructor not found in type \"{type}\".");

        return ctor;
    }

    /// <summary>
    /// Возвращает информацию об указанном свойстве.
    /// </summary>
    /// <remarks>
    /// Бросает исключение, если такое свойство не найдено.
    /// </remarks>
    /// <param name="type"></param>
    /// <param name="propertyName">Название свойства.</param>
    /// <exception cref="MissingMethodException"></exception>
    public static PropertyInfo GetRequiredProperty(this Type type, string propertyName)
    {
        var propertyInfo = type.GetProperty(propertyName);
        if (propertyInfo is null)
            throw new MissingMethodException($"Required property with name \"{propertyName}\" not found in type \"{type}\".");

        return propertyInfo;
    }
}
