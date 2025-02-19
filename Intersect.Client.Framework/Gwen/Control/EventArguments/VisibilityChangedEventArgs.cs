namespace Intersect.Client.Framework.Gwen.Control.EventArguments;

public class VisibilityChangedEventArgs(bool isVisible) : EventArgs
{
    public VisibilityChangedEventArgs(bool isVisibleInParent, bool isVisibleInTree) : this(isVisibleInParent)
    {
        IsVisibleInTree = isVisibleInTree;
    }

    public bool IsVisible { get; init; } = isVisible;

    public bool IsVisibleInTree { get; init; }
}