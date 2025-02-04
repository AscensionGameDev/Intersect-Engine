namespace Intersect.Client.Framework.Gwen;

public enum Orientation
{
    LeftToRight,
    RightToLeft,
    TopToBottom,
    BottomToTop,

    Horizontal = LeftToRight,
    Vertical = TopToBottom,

    Landscape = Horizontal,
    Portrait = Vertical,
}