using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices;
using Intersect.Comparison;
using Intersect.Localization;
using Intersect.Localization.Common;

namespace Intersect.Models.Annotations;

public abstract class ObjectModelAttribute : Attribute
{
}

public record LocalizedStringReference
{
    public LocalizedStringReference(Type namespaceType, string name)
    {
        Name = name;
        NamespaceType = namespaceType;

        FieldInfo = namespaceType.GetField(name, BindingFlags.Public | BindingFlags.Instance) ??
                    throw new ArgumentException(
                        string.Format(
                            ModelAnnotationStrings.LocalizedStringReference_PropertyPDoesNotExistOnTypeT,
                            name,
                            namespaceType.FullName
                        ),
                        nameof(name)
                    );
    }

    public string Name { get; }

    public Type NamespaceType { get; }

    internal FieldInfo FieldInfo { get; }

    public LocalizedString? Get(RootNamespace rootNamespace) => rootNamespace.Localized[FieldInfo] as LocalizedString;

    public static implicit operator LocalizedStringReference((Type, string) tuple)
    {
        var (namespaceType, name) = tuple;
        return namespaceType == default || name == default ? default : new(namespaceType, name);
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class GroupAttribute : ObjectModelAttribute
{
    public GroupAttribute(Type namespaceType, string name)
    {
        Name = (namespaceType, name);
    }

    public LocalizedStringReference Name { get; }
}

public class GroupAttributeComparer : IComparer<GroupAttribute>
{
    public int Compare(GroupAttribute? x, GroupAttribute? y) =>
        x switch
        {
            null when y is null => 0,
            null => 1,
            not null when y is null => -1,
            _ => 0
        };
}

[AttributeUsage(AttributeTargets.Property)]
public class IgnoredAttribute : ObjectModelAttribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class InputAttribute : ObjectModelAttribute
{
    private LocalizedStringReference? _hint;

    public LocalizedStringReference? Hint => _hint ??= (HintNamespace, HintName);

    public string? HintName { get; init; }

    public Type? HintNamespace { get; init; }

    public bool ReadOnly { get; init; }

    public LocalizedString? GetHint(RootNamespace rootNamespace) =>
        Hint == default ? default : rootNamespace.Localized[Hint.FieldInfo] as LocalizedString;
}

[AttributeUsage(AttributeTargets.Property)]
public class InputRangeFloatAttribute : InputAttribute
{
    public InputRangeFloatAttribute() { }

    public InputRangeFloatAttribute(double minimum, double maximum)
    {
        Maximum = maximum;
        Minimum = minimum;
    }

    public double Maximum { get; init; }

    public double Minimum { get; init; }
}

[AttributeUsage(AttributeTargets.Property)]
public class InputRangeIntegralAttribute : InputAttribute
{
    public InputRangeIntegralAttribute() { }

    public InputRangeIntegralAttribute(long minimum, long maximum)
    {
        Maximum = maximum;
        Minimum = minimum;
    }

    public long Maximum { get; init; }

    public long Minimum { get; init; }
}

[AttributeUsage(AttributeTargets.Property)]
public class InputRangeIntegralUnsignedAttribute : InputAttribute
{
    public InputRangeIntegralUnsignedAttribute() { }

    public InputRangeIntegralUnsignedAttribute(ulong minimum, ulong maximum)
    {
        Maximum = maximum;
        Minimum = minimum;
    }

    public ulong Maximum { get; init; }

    public ulong Minimum { get; init; }
}

[AttributeUsage(AttributeTargets.Property)]
public class InputLookupAttribute : InputAttribute
{
    public InputLookupAttribute(Type foreignObjectLookupType, string foreignObjectKeyName, string foreignKeyName)
    {
        ForeignKeyName = foreignKeyName;
        ForeignObjectKeyName = foreignObjectKeyName;
        ForeignObjectLookupType = foreignObjectLookupType;
    }

    public string ForeignKeyName { get; init; }

    public string ForeignObjectKeyName { get; init; }

    public Type ForeignObjectLookupType { get; init; }
}

[AttributeUsage(AttributeTargets.Property)]
public class InputTextAttribute : InputAttribute
{
    public uint MaximumLength { get; init; }
}

public abstract class LocalizedStringReferenceAttribute : ObjectModelAttribute
{
    public LocalizedStringReferenceAttribute(Type @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
        Reference = (Namespace, Name);
    }

    public string Name { get; }

    public Type Namespace { get; }

    public LocalizedStringReference Reference { get; }

    public LocalizedString? Get(RootNamespace rootNamespace) => Reference.Get(rootNamespace);
}

[AttributeUsage(AttributeTargets.Property)]
public class LabelAttribute : LocalizedStringReferenceAttribute
{
    public LabelAttribute(Type @namespace, string name) : base(@namespace, name) { }
}

[AttributeUsage(AttributeTargets.Property)]
public class OrderAttribute : ObjectModelAttribute
{
    public OrderAttribute() { }

    public OrderAttribute(int order) => Order = order;

    public int Order { get; init; }
}

[AttributeUsage(AttributeTargets.Property)]
public class TooltipAttribute : LocalizedStringReferenceAttribute
{
    public TooltipAttribute(Type @namespace, string name) : base(@namespace, name) { }
}
