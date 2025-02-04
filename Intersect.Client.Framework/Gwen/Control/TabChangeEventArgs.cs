namespace Intersect.Client.Framework.Gwen.Control;

public class TabChangeEventArgs : EventArgs
{
    public required TabButton? PreviousTab { get; init; }

    public required TabButton? ActiveTab { get; init; }
}