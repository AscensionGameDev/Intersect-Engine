namespace Intersect.Client.Framework.Gwen.Control.EventArguments;

public class VisibilityChangedEventArgs(bool isVisible) : EventArgs
{
    public bool IsVisible { get; init; } = isVisible;
}