using Intersect.Enums;
using Intersect.GameObjects.Annotations;
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