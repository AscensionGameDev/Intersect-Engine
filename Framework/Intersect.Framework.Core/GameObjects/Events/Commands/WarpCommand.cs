using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Maps;

namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class WarpCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.WarpPlayer;

    public Guid MapId { get; set; }

    public byte X { get; set; }

    public byte Y { get; set; }

    public WarpDirection Direction { get; set; } = WarpDirection.Retain;

    /// <summary>
    /// Whether or not the warp event will change a player's map instance settings
    /// </summary>
    public bool ChangeInstance { get; set; } = false;

    /// <summary>
    /// The <see cref="MapInstanceType"/> we are going to be warping to
    /// </summary>
    public MapInstanceType InstanceType { get; set; } = MapInstanceType.Overworld;
}