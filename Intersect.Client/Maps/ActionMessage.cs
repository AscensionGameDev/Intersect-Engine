using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Utilities;

namespace Intersect.Client.Maps
{

    public partial class ActionMessage : IActionMessage
    {

        public Color Color { get; set; } = new Color();

        public IMapInstance Map { get; set; }

        public string Msg { get; set; } = "";

        public long TransmissionTimer { get; set; }

        public int X { get; set; }

        public long XOffset { get; set; }

        public int Y { get; set; }

        public ActionMessage(MapInstance map, int x, int y, string message, Color color)
        {
            Map = map;
            X = x;
            Y = y;
            Msg = message;
            Color = color;
            XOffset = Globals.Random.Next(-30, 30); //+- 16 pixels so action msg's don't overlap!
            TransmissionTimer = Timing.Global.MillisecondsUtc + 1000;
        }

        public void TryRemove()
        {
            if (TransmissionTimer <= Timing.Global.MillisecondsUtc)
            {
                (Map as MapInstance).ActionMessages.Remove(this);
            }
        }

    }

}
