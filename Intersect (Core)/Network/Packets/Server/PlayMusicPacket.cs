namespace Intersect.Network.Packets.Server
{

    public class PlayMusicPacket : CerasPacket
    {

        public PlayMusicPacket(string bgm)
        {
            BGM = bgm;
        }

        public string BGM { get; set; }

    }

}
