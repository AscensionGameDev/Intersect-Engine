using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class ShowPicturePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public ShowPicturePacket()
        {
        }

        public ShowPicturePacket(string picture, int size, bool clickable, int hideTime, Guid eventId)
        {
            Picture = picture;
            Size = size;
            Clickable = clickable;
            HideTime = hideTime;
            EventId = eventId;
        }

        [Key(0)]
        public string Picture { get; set; }

        [Key(1)]
        public int Size { get; set; }

        [Key(2)]
        public bool Clickable { get; set; }

        [Key(3)]
        public int HideTime { get; set; }

        [Key(4)]
        public Guid EventId { get; set; }

    }

}
