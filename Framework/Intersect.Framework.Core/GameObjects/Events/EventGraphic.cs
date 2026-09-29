using MessagePack;

namespace Intersect.Framework.Core.GameObjects.Events;

[MessagePackObject]
public partial class EventGraphic
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

    // Procedural idle animation - no hand-drawn frames needed, computed client-side from time.
    [Key(6)]
    public IdleMotionType IdleMotionType;

    // Oscillations per second.
    [Key(7)]
    public float IdleMotionSpeed;

    // Pixels for Float/Bounce, degrees for Sway.
    [Key(8)]
    public float IdleMotionAmplitude;

    // For Sway: where the rotation pivot sits, as a percentage of sprite height from the top
    // (0 = anchored at the top, 100 = anchored at the bottom like a tree trunk, 50 = old center-pivot
    // behavior where the whole sprite swings as one rigid piece). Ignored for Float/Bounce.
    [Key(9)]
    public float IdleMotionPivotPercent;

    public EventGraphic()
    {
        Type = EventGraphicType.None;
        Filename = string.Empty;
        X = -1;
        Y = -1;
        Width = -1;
        Height = -1;
        IdleMotionType = IdleMotionType.None;
        IdleMotionSpeed = 1f;
        IdleMotionAmplitude = 4f;
        IdleMotionPivotPercent = 100f;
    }

    public void CopyFrom(EventGraphic toCopy)
    {
        Type = toCopy.Type;
        Filename = toCopy.Filename;
        X = toCopy.X;
        Y = toCopy.Y;
        Width = toCopy.Width;
        Height = toCopy.Height;
        IdleMotionType = toCopy.IdleMotionType;
        IdleMotionSpeed = toCopy.IdleMotionSpeed;
        IdleMotionAmplitude = toCopy.IdleMotionAmplitude;
        IdleMotionPivotPercent = toCopy.IdleMotionPivotPercent;
    }
}
