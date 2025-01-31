using Intersect.Client.Framework.GenericClasses;
using Newtonsoft.Json;

namespace Intersect.Client.Framework.Input;

[method: JsonConstructor]
public partial class ControlBinding(Keys modifier, Keys key)
{
    public static ControlBinding Default => new(Keys.None, Keys.None);

    public Keys Modifier { get; set; } = modifier;

    public Keys Key { get; set; } = key;

    public bool IsMouseKey => Key is Keys.LButton
        or Keys.RButton
        or Keys.MButton
        or Keys.XButton1
        or Keys.XButton2;

    public ControlBinding(ControlBinding controlBinding) : this(controlBinding.Modifier, controlBinding.Key)
    {
    }

    public bool IsDown()
    {
        var gameInput = GameInput.Current;

        // Check to see if our modifier and real key are pressed!
        if (Modifier != Keys.None && !gameInput.IsKeyDown(Modifier))
        {
            return false;
        }

        if (IsMouseKey && Key.TryGetMouseButton(out var mouseButton))
        {
            return gameInput.MouseButtonDown(mouseButton);
        }

        return gameInput.IsKeyDown(Key);
    }
}
