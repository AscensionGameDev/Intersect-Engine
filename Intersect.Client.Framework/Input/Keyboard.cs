namespace Intersect.Client.Framework.Input;

public abstract class Keyboard : Stateful<KeyboardState, Key>
{
    public bool HasModifier(KeyModifiers modifier) => State.HasModifier(modifier);
}
