using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ShowPicturePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ShowPicturePacket()
        {
        }

        public ShowPicturePacket(string picture, int size, bool clickable)
        {
            Picture = picture;
            Size = size;
            Clickable = clickable;
        }

        [Key(0)]
        public string Picture { get; set; }

        [Key(1)]
        public int Size { get; set; }

        [Key(2)]
        public bool Clickable { get; set; }

    }

}
