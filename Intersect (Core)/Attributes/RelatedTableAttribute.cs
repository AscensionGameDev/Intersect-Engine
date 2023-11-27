using Intersect.Enums;

namespace Intersect.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class RelatedTableAttribute : Attribute
{
    public GameObjectType TableType { get; }

    public RelatedTableAttribute(GameObjectType gameObjectType) { TableType = gameObjectType; }
}
