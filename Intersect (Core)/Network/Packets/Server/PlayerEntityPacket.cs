﻿using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class PlayerEntityPacket : EntityPacket
{
    //Parameterless Constructor for MessagePack
    public PlayerEntityPacket()
    {
    }


    [Key(24)]
    public Access AccessLevel { get; set; }


    [Key(25)]
    public Gender Gender { get; set; }


    [Key(26)]
    public Guid ClassId { get; set; }


    [Key(27)]
    public EquipmentPacket Equipment { get; set; }


    [Key(28)]
    public long CombatTimeRemaining { get; set; }


    [Key(29)]
    public string Guild { get; set; }


    [Key(30)]
    public int GuildRank { get; set; }
}
