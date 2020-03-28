namespace Intersect.Network.Packets.Server
{

    public class PlaySoundPacket : CerasPacket
    {

        public PlaySoundPacket(string sound)
        {
            Sound = sound;
        }

        public string Sound { get; set; }

    }

}
