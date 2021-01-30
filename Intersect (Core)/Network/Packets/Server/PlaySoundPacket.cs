using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PlaySoundPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PlaySoundPacket()
        {
        }

        public PlaySoundPacket(string sound)
        {
            Sound = sound;
        }

        [Key(0)]
        public string Sound { get; set; }

    }

}
