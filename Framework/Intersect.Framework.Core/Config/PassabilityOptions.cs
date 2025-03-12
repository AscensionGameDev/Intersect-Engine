using Intersect.Framework.Core.GameObjects.Maps;

namespace Intersect.Config;

public partial class PassabilityOptions : Dictionary<MapZone, bool>
{
    public bool Default { get; set; } = false;

    public PassabilityOptions()
    {
        this[MapZone.Arena] = false;
        this[MapZone.Normal] = false;
        this[MapZone.Safe] = true;
    }

    public bool IsPassable(MapZone mapZoneType) => TryGetValue(mapZoneType, out var passable) ? passable : Default;
}
