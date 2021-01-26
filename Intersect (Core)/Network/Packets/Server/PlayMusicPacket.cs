using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PlayMusicPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PlayMusicPacket()
        {
        }

        public PlayMusicPacket(string bgm)
        {
            BGM = bgm;
        }

        [Key(0)]
        public string BGM { get; set; }

    }

}
