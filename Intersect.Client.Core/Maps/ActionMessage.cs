using Intersect.Client.Framework.Maps;
using Intersect.Client.General;
using Intersect.Utilities;

namespace Intersect.Client.Maps;


public partial class ActionMessage : IActionMessage
{
    public Color Color { get; init; }

    public IMapInstance Map { get; init; }

    public string Text { get; init; }

    public long TransmissionTimer { get; init; }

    public int X { get; init; }

    public int XOffset { get; init; }

    public int Y { get; init; }

    public ActionMessage(MapInstance map, int x, int y, string text, Color color)
    {
        Map = map;
        X = x;
        Y = y;
        Text = text;
        Color = color;
        XOffset = Globals.Random.Next(-30, 30); //+- 16 pixels so action msg's don't overlap!
        TransmissionTimer = Timing.Global.MillisecondsUtc + 1000;
    }

    public void TryRemove()
    {
        if (TransmissionTimer <= Timing.Global.MillisecondsUtc)
        {
            (Map as MapInstance)?.ActionMessages.Remove(this);
        }
    }

}
