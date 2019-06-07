using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.Network.Packets.Server
{
    public class StatusPacket : CerasPacket
    {
        public Guid SpellId { get; set; }
        public StatusTypes Type { get; set; }
        public string TransformSprite { get; set; }
        public long TimeRemaining { get; set; }
        public long TotalDuration { get; set; }
        public int[] VitalShields { get; set; }

        public StatusPacket(Guid spellId, StatusTypes type, string transformSprite, long timeRemaining, long totalDuration, int[] vitalShields)
        {
            SpellId = spellId;
            Type = type;
            TransformSprite = transformSprite;
            TimeRemaining = timeRemaining;
            TotalDuration = totalDuration;
            VitalShields = vitalShields;
        }
    }
}
