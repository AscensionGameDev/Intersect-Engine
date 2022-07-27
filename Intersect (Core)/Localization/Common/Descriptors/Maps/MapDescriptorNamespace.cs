using Intersect.Enums;

using Newtonsoft.Json;

namespace Intersect.Localization.Common.Descriptors.Maps;

public partial class MapDescriptorNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocaleDictionary<MapAttributes, LocalizedString> AttributeTypes = new()
    {
        { MapAttributes.Animation, @"Animation" },
        { MapAttributes.Blocked, @"Blocked" },
        { MapAttributes.Critter, @"Critter" },
        { MapAttributes.GrappleStone, @"Grapple Stone" },
        { MapAttributes.Item, @"Item Spawn" },
        { MapAttributes.NpcAvoid, @"Npc Avoid" },
        { MapAttributes.Resource, @"Resource Spawn" },
        { MapAttributes.Slide, @"Slide" },
        { MapAttributes.Sound, @"Map Sound" },
        { MapAttributes.Walkable, @"Walkable" },
        { MapAttributes.Warp, @"Warp" },
        { MapAttributes.ZDimension, @"Z-Dimension" },
    };
}
