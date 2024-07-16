using Intersect.Client.General;
using Intersect.Client.Framework.GenericClasses;
using Intersect.Client.Framework.Input;
using Newtonsoft.Json;

namespace Intersect.Client.Core.Controls;

public partial class ControlValue
{
    public static ControlValue Default => new(Keys.None, Keys.None);

    public Keys Modifier { get; set; }

    public Keys Key { get; set; }

    public bool IsMouseKey => Key is Keys.LButton
        or Keys.RButton
        or Keys.MButton
        or Keys.XButton1
        or Keys.XButton2;

    [JsonConstructor]
    public ControlValue(Keys modifier, Keys key)
    {
        Modifier = modifier;
        Key = key;
    }

    public ControlValue(ControlValue controlValue)
    {
        Modifier = controlValue.Modifier;
        Key = controlValue.Key;
    }

    public bool IsDown()
    {
        // Assume our modifier is clicked.
        var modifier = true;

        // Check to see if it actually is, if not disable it!
        if (Modifier != Keys.None && !KeyDown(Modifier))
        {
            modifier = false;
        }

        // Check to see if our modifier and real key are pressed!
        if (modifier)
        {
            if (IsMouseKey)
            {
                switch (Key)
                {
                    case Keys.LButton:
                        if (Globals.InputManager.MouseButtonDown(MouseButtons.Left))
                        {
                            return true;
                        }

                        break;
                    case Keys.RButton:
                        if (Globals.InputManager.MouseButtonDown(MouseButtons.Right))
                        {
                            return true;
                        }

                        break;
                    case Keys.MButton:
                        if (Globals.InputManager.MouseButtonDown(MouseButtons.Middle))
                        {
                            return true;
                        }

                        break;
                    case Keys.XButton1:
                        if (Globals.InputManager.MouseButtonDown(MouseButtons.X1))
                        {
                            return true;
                        }

                        break;
                    case Keys.XButton2:
                        if (Globals.InputManager.MouseButtonDown(MouseButtons.X2))
                        {
                            return true;
                        }

                        break;
                }
            }
            else
            {
                if (KeyDown(Key))
                {
                    return true;
                }
            }
        }
       
        // If we get this far, clearly our buttons aren't pressed.
        return false;
    }

    private static bool KeyDown(Keys key) => Globals.InputManager.KeyDown(key);
}
