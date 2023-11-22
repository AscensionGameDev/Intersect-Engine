using System.Reflection;
using Intersect.Attributes;
using Intersect.Enums;

namespace Intersect.Extensions;
public static class VariableTypeExtensions
{
    public static GameObjectType GetRelatedTable(this VariableType value)
    {
        if (!Enum.IsDefined(typeof(GameObjectType), value))
        {
            throw new ArgumentException($"{nameof(value)} is not a valid VariableType enum");
        }

        string name = Enum.GetName(typeof(VariableType), value);
        if (name == null)
        {
            throw new ArgumentException($"{nameof(value)} had an empty enum name");
        }

        FieldInfo fieldInfo = typeof(VariableType).GetField(name);
        if (fieldInfo == null)
        {
            throw new ArgumentException($"Reflection failed for GameObjectType enum {nameof(value)}");
        }

        RelatedTableAttribute attr = fieldInfo.GetCustomAttribute<RelatedTableAttribute>();
        if (attr == null)
        {
            throw new ArgumentException($"Failed to get RelatedTable attribute for VariableType enum {nameof(value)}");
        }

        return attr.TableType;
    }
}
