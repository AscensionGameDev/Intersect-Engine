using Intersect.Enums;

namespace Intersect.GameObjects.Maps;

public partial class MapBlockedAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Blocked;
}