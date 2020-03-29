namespace Intersect.Network.Packets.Server
{

    public class ShowPicturePacket : CerasPacket
    {

        public ShowPicturePacket(string picture, int size, bool clickable)
        {
            Picture = picture;
            Size = size;
            Clickable = clickable;
        }

        public string Picture { get; set; }

        public int Size { get; set; }

        public bool Clickable { get; set; }

    }

}
