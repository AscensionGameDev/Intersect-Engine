using Intersect.Enums;
using MessagePack;

namespace Intersect.GameObjects.Events
{
    [MessagePackObject]
    public class EventGraphic
    {
        [Key(0)]
        public string Filename;
        [Key(1)]
        public int Height;
        [Key(2)]
        public EventGraphicType Type;
        [Key(3)]
        public int Width;
        [Key(4)]
        public int X;
        [Key(5)]
        public int Y;

        public EventGraphic()
        {
            Type = EventGraphicType.None;
            Filename = "";
            X = -1;
            Y = -1;
            Width = -1;
            Height = -1;
        }

        public void CopyFrom(EventGraphic toCopy)
        {
            Type = toCopy.Type;
            Filename = toCopy.Filename;
            X = toCopy.X;
            Y = toCopy.Y;
            Width = toCopy.Width;
            Height = toCopy.Height;
        }

    }

}
