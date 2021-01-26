using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ExperiencePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ExperiencePacket()
        {
        }

        public ExperiencePacket(long exp, long tnl)
        {
            Experience = exp;
            ExperienceToNextLevel = tnl;
        }

        [Key(0)]
        public long Experience { get; set; }

        [Key(1)]
        public long ExperienceToNextLevel { get; set; }

    }

}
