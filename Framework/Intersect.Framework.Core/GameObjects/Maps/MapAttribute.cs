using Intersect.Enums;
using Intersect.GameObjects.Annotations;
using Intersect.Localization;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Maps;

public abstract partial class MapAttribute
{
    [EditorLabel("Attributes", "AttributeType")]
    [EditorDictionary("Attributes", "AttributeTypes", FieldType = EditorFieldType.Pivot)]
    public abstract MapAttributeType Type { get; }

    public static MapAttribute CreateAttribute(MapAttributeType type)
    {
        switch (type)
        {
            case MapAttributeType.Walkable:
                return null;
            case MapAttributeType.Blocked:
                return new MapBlockedAttribute();
            case MapAttributeType.Item:
                return new MapItemAttribute();
            case MapAttributeType.ZDimension:
                return new MapZDimensionAttribute();
            case MapAttributeType.NpcAvoid:
                return new MapNpcAvoidAttribute();
            case MapAttributeType.Warp:
                return new MapWarpAttribute();
            case MapAttributeType.Sound:
                return new MapSoundAttribute();
            case MapAttributeType.Resource:
                return new MapResourceAttribute();
            case MapAttributeType.Animation:
                return new MapAnimationAttribute();
            case MapAttributeType.GrappleStone:
                return new MapGrappleStoneAttribute();
            case MapAttributeType.Slide:
                return new MapSlideAttribute();
            case MapAttributeType.Critter:
                return new MapCritterAttribute();
        }

        return null;
    }

    public virtual MapAttribute Clone()
    {
        return CreateAttribute(this.Type);
    }

    public string Data()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public partial class MapBlockedAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Blocked;
}

public partial class MapItemAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Item;

    [EditorLabel("Attributes", "Item")]
    [EditorReference(typeof(ItemBase), nameof(ItemBase.Name))]
    public Guid ItemId { get; set; }

    [EditorLabel("Attributes", "Quantity")]
    [EditorDisplay]
    public int Quantity { get; set; }

    public long RespawnTime { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapItemAttribute) base.Clone();
        att.ItemId = ItemId;
        att.Quantity = Quantity;
        att.RespawnTime = RespawnTime;

        return att;
    }
}

public partial class MapZDimensionAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.ZDimension;

    [EditorLabel("Attributes", "ZGateway")]
    [EditorFormatted("Attributes", "FormatZLevel")]
    public byte GatewayTo { get; set; }

    [EditorLabel("Attributes", "ZBlock")]
    [EditorFormatted("Attributes", "FormatZLevel")]
    public byte BlockedLevel { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapZDimensionAttribute) base.Clone();
        att.GatewayTo = GatewayTo;
        att.BlockedLevel = BlockedLevel;

        return att;
    }
}

public partial class MapNpcAvoidAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.NpcAvoid;
}

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

public partial class MapSoundAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Sound;

    [EditorLabel("Attributes", "Sound")]
    [EditorDisplay(
        EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty,
        StringBehavior = StringBehavior.Trim
    )]
    public string File { get; set; }

    [EditorLabel("Attributes", "SoundDistance")]
    [EditorFormatted("Attributes", "DistanceFormat")]
    public byte Distance { get; set; }

    [EditorLabel("Attributes", "SoundInterval")]
    [EditorTime]
    public int LoopInterval { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapSoundAttribute) base.Clone();
        att.File = File;
        att.Distance = Distance;
        att.LoopInterval = LoopInterval;

        return att;
    }
}

public partial class MapResourceAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Resource;

    [EditorLabel("Attributes", "Resource")]
    [EditorReference(typeof(ResourceBase), nameof(ResourceBase.Name))]
    public Guid ResourceId { get; set; }

    [EditorLabel("Attributes", "ZDimension")]
    [EditorFormatted("Attributes", "FormatSpawnLevel")]
    public byte SpawnLevel { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapResourceAttribute) base.Clone();
        att.ResourceId = ResourceId;
        att.SpawnLevel = SpawnLevel;

        return att;
    }
}

public partial class MapAnimationAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Animation;

    [EditorLabel("Attributes", "MapAnimation")]
    [EditorReference(typeof(AnimationDescriptor), nameof(AnimationDescriptor.Name))]
    public Guid AnimationId { get; set; }

    [EditorLabel("Attributes", "MapAnimationBlock")]
    [EditorBoolean(Style = BooleanStyle.YesNo)]
    public bool IsBlock { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapAnimationAttribute) base.Clone();
        att.AnimationId = AnimationId;
        att.IsBlock = IsBlock;

        return att;
    }
}

public partial class MapGrappleStoneAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.GrappleStone;
}

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

public partial class MapCritterAttribute : MapAttribute
{
    public override MapAttributeType Type => MapAttributeType.Critter;

    [EditorLabel("Attributes", "CritterSprite")]
    [EditorDisplay(
        EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty,
        StringBehavior = StringBehavior.Trim
    )]
    public string Sprite { get; set; }

    [EditorLabel("Attributes", "CritterAnimation")]
    [EditorReference(typeof(AnimationDescriptor), nameof(AnimationDescriptor.Name))]
    public Guid AnimationId { get; set; }

    //Movement types will mimic npc options?
    //Random
    //Turn
    //Still
    [EditorLabel("Attributes", "CritterMovement")]
    [EditorDictionary("Attributes", "CritterMovements")]
    public byte Movement { get; set; }

    //Time in MS to traverse a tile once moving
    [EditorLabel("Attributes", "CritterSpeed")]
    [EditorTime]
    public int Speed { get; set; }

    //Time in MS between movements?
    [EditorLabel("Attributes", "CritterFrequency")]
    [EditorTime]
    public int Frequency { get; set; }

    //Lower, Middle, Upper
    [EditorLabel("Attributes", "CritterLayer")]
    [EditorDictionary("Attributes", "CritterLayers")]
    public byte Layer { get; set; }

    [EditorLabel("Attributes", "CritterDirection")]
    [EditorDictionary(nameof(Direction), "CritterDirection")]
    public byte Direction { get; set; }

    [EditorLabel("Attributes", "CritterIgnoreNpcAvoids")]
    [EditorBoolean(Style = BooleanStyle.YesNo)]
    public bool IgnoreNpcAvoids { get; set; }

    [EditorLabel("Attributes", "CritterBlockPlayers")]
    [EditorBoolean(Style = BooleanStyle.YesNo)]
    public bool BlockPlayers { get; set; }

    public override MapAttribute Clone()
    {
        var att = (MapCritterAttribute)base.Clone();
        att.Sprite = Sprite;
        att.AnimationId = AnimationId;
        att.Movement = Movement;
        att.Speed = Speed;
        att.Frequency = Frequency;
        att.Layer = Layer;
        att.IgnoreNpcAvoids = IgnoreNpcAvoids;

        return att;
    }
}
