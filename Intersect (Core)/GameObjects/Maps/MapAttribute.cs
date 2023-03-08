using System;

using Intersect.Enums;
using Intersect.GameObjects.Annotations;
using Intersect.Localization;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Maps
{
    public abstract partial class MapAttribute
    {
        [EditorLabel("Attributes", "AttributeType")]
        [EditorDictionary("Attributes", "AttributeTypes", FieldType = EditorFieldType.Pivot)]
        public abstract Enums.MapAttribute Type { get; }

        public static MapAttribute CreateAttribute(Enums.MapAttribute type)
        {
            switch (type)
            {
                case Enums.MapAttribute.Walkable:
                    return null;
                case Enums.MapAttribute.Blocked:
                    return new MapBlockedAttribute();
                case Enums.MapAttribute.Item:
                    return new MapItemAttribute();
                case Enums.MapAttribute.ZDimension:
                    return new MapZDimensionAttribute();
                case Enums.MapAttribute.NpcAvoid:
                    return new MapNpcAvoidAttribute();
                case Enums.MapAttribute.Warp:
                    return new MapWarpAttribute();
                case Enums.MapAttribute.Sound:
                    return new MapSoundAttribute();
                case Enums.MapAttribute.Resource:
                    return new MapResourceAttribute();
                case Enums.MapAttribute.Animation:
                    return new MapAnimationAttribute();
                case Enums.MapAttribute.GrappleStone:
                    return new MapGrappleStoneAttribute();
                case Enums.MapAttribute.Slide:
                    return new MapSlideAttribute();
                case Enums.MapAttribute.Critter:
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
        public override Enums.MapAttribute Type => Enums.MapAttribute.Blocked;
    }

    public partial class MapItemAttribute : MapAttribute
    {
        public override Enums.MapAttribute Type => Enums.MapAttribute.Item;

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
        public override Enums.MapAttribute Type => Enums.MapAttribute.ZDimension;

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
        public override Enums.MapAttribute Type => Enums.MapAttribute.NpcAvoid;
    }

    public partial class MapWarpAttribute : MapAttribute
    {
        public override Enums.MapAttribute Type => Enums.MapAttribute.Warp;

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
        [EditorDictionary("Mapping", "InstanceTypes")]
        public MapInstanceType InstanceType { get; set; } = MapInstanceType.Overworld;

        [EditorLabel("Attributes", "WarpSound")]
        [EditorDisplay(EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty, StringBehavior = StringBehavior.Trim)]
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
        public override Enums.MapAttribute Type => Enums.MapAttribute.Sound;

        [EditorLabel("Attributes", "Sound")]
        [EditorDisplay(EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty, StringBehavior = StringBehavior.Trim)]
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
        public override Enums.MapAttribute Type => Enums.MapAttribute.Resource;

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
        public override Enums.MapAttribute Type => Enums.MapAttribute.Animation;

        [EditorLabel("Attributes", "MapAnimation")]
        [EditorReference(typeof(AnimationBase), nameof(AnimationBase.Name))]
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
        public override Enums.MapAttribute Type => Enums.MapAttribute.GrappleStone;
    }

    public partial class MapSlideAttribute : MapAttribute
    {
        public override Enums.MapAttribute Type => Enums.MapAttribute.Slide;

        [EditorLabel("Attributes", "Direction")]
        [EditorDictionary(nameof(Direction), "WarpDirections")]
        public byte Direction { get; set; }

        public override MapAttribute Clone()
        {
            var att = (MapSlideAttribute) base.Clone();
            att.Direction = Direction;

            return att;
        }
    }

    public partial class MapCritterAttribute : MapAttribute
    {
        public override Enums.MapAttribute Type => Enums.MapAttribute.Critter;

        [EditorLabel("Attributes", "CritterSprite")]
        [EditorDisplay(EmptyBehavior = EmptyBehavior.ShowNoneOnNullOrEmpty, StringBehavior = StringBehavior.Trim)]
        public string Sprite { get; set; }

        [EditorLabel("Attributes", "CritterAnimation")]
        [EditorReference(typeof(AnimationBase), nameof(AnimationBase.Name))]
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

}
