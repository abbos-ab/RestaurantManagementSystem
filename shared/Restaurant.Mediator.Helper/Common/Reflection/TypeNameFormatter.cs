using System.Collections.Concurrent;
using System.Text;

namespace Restaurant.Mediator.Helper.Common.Reflection;

public class TypeNameFormatter
{
    private readonly ConcurrentDictionary<Type, string> _cache;

    private readonly string _genericArgumentSeparator;
    private readonly string _genericClose;
    private readonly string _genericOpen;
    private readonly string _namespaceSeparator;
    private readonly string _nestedTypeSeparator;

    public TypeNameFormatter()
        : this(",", "<", ">", ".", "+") { }

    public TypeNameFormatter(
        string genericArgumentSeparator,
        string genericClose,
        string genericOpen,
        string namespaceSeparator,
        string nestedTypeSeparator)
    {
        _cache = new ConcurrentDictionary<Type, string>();

        _genericArgumentSeparator = genericArgumentSeparator;
        _genericClose = genericClose;
        _genericOpen = genericOpen;
        _namespaceSeparator = namespaceSeparator;
        _nestedTypeSeparator = nestedTypeSeparator;
    }

    public string GetTypeName(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return _cache.GetOrAdd(type, FormatTypeName);
    }

    private string FormatTypeName(Type type)
    {
        if (type.IsGenericTypeDefinition)
            throw new ArgumentException("An open generic type cannot be used as a message name.");

        var sb = new StringBuilder("");

        return FormatTypeName(sb, type, null);
    }

    private string FormatTypeName(StringBuilder sb, Type type, string? scope)
    {
        if (type.IsGenericParameter)
            return "";

        if (type.Namespace != null)
        {
            var ns = type.Namespace;
            if (!ns.Equals(scope))
            {
                sb.Append(ns);
                sb.Append(_namespaceSeparator);
            }
        }

        if (type is { IsNested: true, DeclaringType: not null })
        {
            FormatTypeName(sb, type.DeclaringType, type.Namespace);
            sb.Append(_nestedTypeSeparator);
        }

        if (type.IsGenericType)
        {
            var name = type.GetGenericTypeDefinition().Name;

            //remove `1
            var index = name.IndexOf('`');
            if (index > 0)
                name = name.Remove(index);

            sb.Append(name);
            sb.Append(_genericOpen);
            var arguments = type.GenericTypeArguments;
            for (var i = 0; i < arguments.Length; i++)
            {
                if (i > 0)
                    sb.Append(_genericArgumentSeparator);

                FormatTypeName(sb, arguments[i], type.Namespace);
            }

            sb.Append(_genericClose);
        }
        else
        {
            sb.Append(type.Name);
        }

        return sb.ToString();
    }
}
