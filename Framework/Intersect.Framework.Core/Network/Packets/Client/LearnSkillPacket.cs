using System;
using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class LearnSkillPacket : IntersectPacket
    {
        public LearnSkillPacket(Guid spellId)
        {
            SpellId = spellId;
        }

        public LearnSkillPacket()
        {
        }

        [Key(0)]
        public Guid SpellId { get; set; }
    }
}
