namespace Intersect.Network.Packets.Server
{

    public class ExperiencePacket : CerasPacket
    {

        public ExperiencePacket(long exp, long tnl)
        {
            Experience = exp;
            ExperienceToNextLevel = tnl;
        }

        public long Experience { get; set; }

        public long ExperienceToNextLevel { get; set; }

    }

}
