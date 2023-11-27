using System.Reflection;
using Intersect.Attributes;
using Intersect.Enums;

namespace Intersect.Extensions;
public static class VariableModExtensions
{
    public static VariableType GetRelatedVariableType(this VariableMod value)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid VariableMod enum");
        }

        string name = Enum.GetName(value);
        if (name == null)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Missing enum name");
        }

        FieldInfo fieldInfo = typeof(VariableMod).GetField(name);
        if (fieldInfo == null)
        {
            throw new MissingFieldException($"Reflection failed for VariableType enum, value was {value}", nameof(value));
        }

        RelatedVariableTypeAttribute attr = fieldInfo.GetCustomAttribute<RelatedVariableTypeAttribute>();
        if (attr == null)
        {
            throw new ArgumentException($"Failed to get RelatedVariableType attribute for VariableType enum, value was {value}", nameof(value));
        }

        return attr.VariableType;
    }
}
