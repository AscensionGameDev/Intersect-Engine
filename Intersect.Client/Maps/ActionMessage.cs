using Intersect.Client.Framework.Maps;
using Intersect.Client.General;

namespace Intersect.Client.Maps
{

    public partial class ActionMessage : IActionMessage
    {

        public Color Clr { get; set; } = new Color();

        public IMapInstance Map { get; set; }

        public string Msg { get; set; } = "";

        public long TransmittionTimer { get; set; }

        public int X { get; set; }

        public long XOffset { get; set; }

        public int Y { get; set; }

        public ActionMessage(MapInstance map, int x, int y, string message, Color color)
        {
            Map = map;
            X = x;
            Y = y;
            Msg = message;
            Clr = color;
            XOffset = Globals.Random.Next(-30, 30); //+- 16 pixels so action msg's don't overlap!
            TransmittionTimer = Globals.System.GetTimeMs() + 1000;
        }

        public void TryRemove()
        {
            if (TransmittionTimer <= Globals.System.GetTimeMs())
            {
                Map.ActionMsgs.Remove(this);
            }
        }

    }

}
