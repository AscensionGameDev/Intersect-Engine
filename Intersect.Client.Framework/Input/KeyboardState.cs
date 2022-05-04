namespace Intersect.Client.Framework.Input;

public struct KeyboardState : IIndexableState<Key>
{
    public readonly ButtonState[] Keys;

    public readonly KeyModifiers Modifiers;

    public KeyboardState(
        ButtonState[] keys,
        KeyModifiers keyModifiers
    )
    {
        Keys = keys ?? throw new ArgumentNullException(nameof(keys));
        Modifiers = keyModifiers;
    }

    public ButtonState this[Key key] => Keys?[(int)key] ?? ButtonState.Released;

    public bool HasModifier(KeyModifiers modifier)
    {
        switch (modifier)
        {
            case KeyModifiers.None:
                return modifier == KeyModifiers.None;

            case KeyModifiers.LeftShift:
            case KeyModifiers.RightShift:
            case KeyModifiers.LeftCtrl:
            case KeyModifiers.RightCtrl:
            case KeyModifiers.LeftAlt:
            case KeyModifiers.RightAlt:
            case KeyModifiers.LeftGui:
            case KeyModifiers.RightGui:
            case KeyModifiers.NumLock:
            case KeyModifiers.CapsLock:
            case KeyModifiers.AltGr:
            case KeyModifiers.Reserved:
                return Modifiers.HasFlag(modifier);

            case KeyModifiers.Ctrl:
            case KeyModifiers.Shift:
            case KeyModifiers.Alt:
            case KeyModifiers.Gui:
                return (Modifiers & modifier) != 0;

            default:
                throw new ArgumentOutOfRangeException(nameof(modifier));
        }
    }
}
