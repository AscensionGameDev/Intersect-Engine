namespace Intersect.Client.Framework.Input;

public struct MouseState : IIndexableState<MouseButton>
{
    public readonly int X;

    public readonly int Y;

    public readonly int ScrollX;

    public readonly int ScrollY;

    public readonly ButtonState[] Buttons;

    public MouseState(
        int x,
        int y,
        int scrollX,
        int scrollY,
        ButtonState[] buttons
    )
    {
        X = x;
        Y = y;
        ScrollX = scrollX;
        ScrollY = scrollY;
        Buttons = buttons;
    }

    public ButtonState this[MouseButton mouseButton] => Buttons?[(int)mouseButton] ?? ButtonState.Released;

    public ButtonState Middle => this[MouseButton.Middle];

    public ButtonState Mouse4 => this[MouseButton.Mouse4];

    public ButtonState Mouse5 => this[MouseButton.Mouse5];

    public ButtonState Primary => this[MouseButton.Primary];

    public ButtonState Secondary => this[MouseButton.Secondary];
}
