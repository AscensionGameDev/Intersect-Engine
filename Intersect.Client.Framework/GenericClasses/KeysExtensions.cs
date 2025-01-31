using Intersect.Client.Framework.Input;

namespace Intersect.Client.Framework.GenericClasses;

public static class KeysExtensions
{
    public static bool TryGetMouseButton(this Keys key, out MouseButtons mouseButton)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (key)
        {
            case Keys.LButton:
                mouseButton = MouseButtons.Left;
                return true;

            case Keys.RButton:
                mouseButton = MouseButtons.Right;
                return true;

            case Keys.MButton:
                mouseButton = MouseButtons.Middle;
                return true;

            case Keys.XButton1:
                mouseButton = MouseButtons.X1;
                return true;

            case Keys.XButton2:
                mouseButton = MouseButtons.X2;
                return true;

            default:
                mouseButton = default;
                return false;
        }
    }

    public static string GetKeyId(this Keys key, bool isModifier = false) =>
        Enum.GetName(key) ??
        throw new ArgumentException(
            $"No name found for {(isModifier ? "modifier" : "key")} value {(int)key}",
            nameof(key)
        );
}