using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class StatusPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public StatusPacket()
        {
        }

        public StatusPacket(
            Guid spellId,
            StatusTypes type,
            string transformSprite,
            long timeRemaining,
            long totalDuration,
            int[] vitalShields
        )
        {
            SpellId = spellId;
            Type = type;
            TransformSprite = transformSprite;
            TimeRemaining = timeRemaining;
            TotalDuration = totalDuration;
            VitalShields = vitalShields;
        }

        [Key(0)]
        public Guid SpellId { get; set; }

        [Key(1)]
        public StatusTypes Type { get; set; }

        [Key(2)]
        public string TransformSprite { get; set; }

        [Key(3)]
        public long TimeRemaining { get; set; }

        [Key(4)]
        public long TotalDuration { get; set; }

        [Key(5)]
        public int[] VitalShields { get; set; }

    }

}
