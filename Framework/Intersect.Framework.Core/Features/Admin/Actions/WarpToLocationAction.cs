﻿using MessagePack;

namespace Intersect.Admin.Actions;

[MessagePackObject]
public partial class WarpToLocationAction : AdminAction
{
    //Parameterless Constructor for MessagePack
    public WarpToLocationAction()
    {

    }

    public WarpToLocationAction(Guid mapId, byte x, byte y)
    {
        MapId = mapId;
        X = x;
        Y = y;
    }

    [Key(1)]
    public override Enums.AdminAction Action { get; } = Enums.AdminAction.WarpTo;

    [Key(2)]
    public Guid MapId { get; set; }

    [Key(3)]
    public byte X { get; set; }

    [Key(4)]
    public byte Y { get; set; }
}
