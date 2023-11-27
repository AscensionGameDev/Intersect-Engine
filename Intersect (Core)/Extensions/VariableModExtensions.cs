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
            throw new ArgumentException("Invalid VariableMod enum", nameof(value));
        }

        string name = Enum.GetName(value);
        if (name == null)
        {
            throw new ArgumentException("Missing enum name", nameof(value));
        }

        FieldInfo fieldInfo = typeof(VariableMod).GetField(name);
        if (fieldInfo == null)
        {
            throw new MissingFieldException("Reflection failed for VariableType enum", nameof(value));
        }

        RelatedVariableTypeAttribute attr = fieldInfo.GetCustomAttribute<RelatedVariableTypeAttribute>();
        if (attr == null)
        {
            throw new ArgumentException("Failed to get RelatedVariableType attribute for VariableType enum", nameof(value));
        }

        return attr.VariableType;
    }
}
