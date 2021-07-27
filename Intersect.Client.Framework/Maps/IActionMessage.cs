namespace Intersect.Client.Framework.Maps
{
    public interface IActionMessage
    {
        Color Clr { get; set; }
        IMapInstance Map { get; set; }
        string Msg { get; set; }
        long TransmittionTimer { get; set; }
        int X { get; set; }
        long XOffset { get; set; }
        int Y { get; set; }

        void TryRemove();
    }
}