using Intersect.Enums;
using Intersect.GameObjects.Annotations;
using Intersect.Localization;

namespace Intersect.GameObjects.Maps;

public partial class MapWarpAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Warp;

    [EditorLabel("Attributes", "Map")]
    [EditorReference(typeof(MapBase), nameof(MapBase.Name))]
    public Guid MapId { get; set; }

    [EditorLabel("Attributes", "WarpX")]
    [EditorDisplay]
    public byte X { get; set; }

    [EditorLabel("Attributes", "WarpY")]
    [EditorDisplay]
    public byte Y { get; set; }

    [EditorLabel("Attributes", "WarpDirection")]
    [EditorDictionary(nameof(Direction), "WarpDirections")]
    public WarpDirection Direction { get; set; } = WarpDirection.Retain;

    [EditorLabel("Warping", "ChangeInstance")]
    [EditorBoolean(Style = BooleanStyle.YesNo)]
    public bool ChangeInstance { get; set; } = false;

    [EditorLabel("Warping", "InstanceType")]
    [EditorDictionary("MapInstance", "InstanceTypes")]
    public MapInstanceType InstanceType { get; set; } = MapInstanceType.Overworld;

    [EditorLabel("Attributes", "WarpSound")]
    [EditorDisplay(
        EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty,
        StringBehavior = StringBehavior.Trim
    )]
    public string WarpSound { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapWarpAttribute) base.Clone();
        att.MapId = MapId;
        att.X = X;
        att.Y = Y;
        att.Direction = Direction;
        att.ChangeInstance = ChangeInstance;
        att.InstanceType = InstanceType;
        att.WarpSound = WarpSound;
        return att;
    }
}