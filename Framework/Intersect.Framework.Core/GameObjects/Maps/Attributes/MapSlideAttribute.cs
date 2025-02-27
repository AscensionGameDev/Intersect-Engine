using Intersect.Enums;
using Intersect.GameObjects.Annotations;

namespace Intersect.GameObjects.Maps;

public partial class MapSlideAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Slide;

    [EditorLabel("Attributes", "Direction")]
    [EditorDictionary(nameof(Direction), "WarpDirections")]
    public Direction Direction { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapSlideAttribute) base.Clone();
        att.Direction = Direction;

        return att;
    }
}