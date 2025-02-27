using Intersect.Enums;

namespace Intersect.GameObjects.Events.Commands;

public partial class SpawnNpcCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SpawnNpc;

    public Guid NpcId { get; set; }

    public Direction Dir { get; set; }

    //Tile Spawn Variables  (Will spawn on map tile if mapid is not empty)
    public Guid MapId { get; set; }

    //Entity Spawn Variables (Will spawn on/around entity if entityId is not empty)
    public Guid EntityId { get; set; }

    //Map Coords or Coords Centered around player to spawn at
    public sbyte X { get; set; }

    public sbyte Y { get; set; }
}