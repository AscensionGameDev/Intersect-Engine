using Intersect.Enums;
using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PlayerStatsPacket : EntityStatsPacket
    {
        PlayerStatsPacket() { } // MP

        public PlayerStatsPacket(Guid id, EntityTypes type, Guid mapId, int[] stats, int[] trueStats) : base(id, type, mapId, stats)
        {
            TrueStats = trueStats;
        }
        
        [Key(4)]
        public int[] TrueStats { get; set; }
    }
}
