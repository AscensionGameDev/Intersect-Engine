using Intersect.Enums;

namespace Intersect.GameObjects.Events
{

    public class EventGraphic
    {

        public string Filename;

        public int Height;

        public EventGraphicType Type;

        public int Width;

        public int X;

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
