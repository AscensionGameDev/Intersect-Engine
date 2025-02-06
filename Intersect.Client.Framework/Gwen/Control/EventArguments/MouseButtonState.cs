using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.Gwen.Control.EventArguments;

public sealed class MouseButtonState : EventArgs
{
    internal MouseButtonState(MouseButton mouseButton, int x, int y, bool isPressed)
    {
        MouseButton = mouseButton;
        X = x;
        Y = y;
        IsPressed = isPressed;
    }

    internal MouseButtonState(MouseButton mouseButton, Point mousePosition, bool isPressed)
    {
        MouseButton = mouseButton;
        X = mousePosition.X;
        Y = mousePosition.Y;
        IsPressed = isPressed;
    }

    public MouseButton MouseButton { get; }

    public int X { get; private set; }

    public int Y { get; private set; }

    public bool IsPressed { get; private set; }
}