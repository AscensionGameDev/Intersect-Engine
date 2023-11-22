using Intersect.Enums;

namespace Intersect.Attributes;
public sealed class RelatedTableAttribute : Attribute
{
    public GameObjectType TableType { get; set; }

    public RelatedTableAttribute(GameObjectType db) { TableType = db; }
}
