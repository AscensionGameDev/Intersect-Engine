namespace Intersect.Client.Framework.Maps;

public interface IActionMessage
{
    Color Color { get; init; }
    IMapInstance Map { get; init; }
    string Text { get; init; }
    long TransmissionTimer { get; init; }
    int X { get; init; }
    int XOffset { get; init; }
    int Y { get; init; }

    void TryRemove();
}