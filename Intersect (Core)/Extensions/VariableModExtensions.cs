using System.Reflection;
using Intersect.Attributes;
using Intersect.Enums;

namespace Intersect.Extensions;
public static class VariableModExtensions
{
    public static VariableType GetRelatedVariableType(this VariableMod value)
    {
        if (!Enum.IsDefined(typeof(VariableMod), value))
        {
            throw new ArgumentException($"{nameof(value)} is not a valid VariableMod enum");
        }

        string name = Enum.GetName(typeof(VariableMod), value);
        if (name == null)
        {
            throw new ArgumentException($"{nameof(value)} had an empty enum name");
        }

        FieldInfo fieldInfo = typeof(VariableMod).GetField(name);
        if (fieldInfo == null)
        {
            throw new ArgumentException($"Reflection failed for VariableType enum {nameof(value)}");
        }

        RelatedVariableTypeAttribute attr = fieldInfo.GetCustomAttribute<RelatedVariableTypeAttribute>();
        if (attr == null)
        {
            throw new ArgumentException($"Failed to get RelatedVariableType attribute for VariableType enum {nameof(value)}");
        }


        return attr.VariableType;
    }
}
