using Intersect.Config;

namespace Intersect.Client.Framework.Input;

public static class ControlExtensions
{
    public static string GetControlId(this Control control)
    {
        var hotkeyValue = control - Control.HotkeyOffset;
        var hotbarSlotCount = Options.Instance?.Player.HotbarSlotCount ?? PlayerOptions.DefaultHotbarSlotCount;
        if (10 < hotkeyValue && hotkeyValue <= hotbarSlotCount)
        {
            return $"Hotkey{hotkeyValue}";
        }

        var controlName = Enum.GetName(control);
        if (!string.IsNullOrWhiteSpace(controlName))
        {
            return controlName;
        }

        throw new ArgumentException($"No name found for control value {(int)control}", nameof(control));
    }

    public static bool IsValid(this Control control)
    {
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        // Using a switch assuming that we will add more configurable-size control regions,
        // where the offsets are always invalid, while other controls may be valid
        // ultimately depending on the exact configuration.
        switch (control)
        {
            case Control.HotkeyOffset:
                return false;
        }

        // ReSharper disable once InvertIf
        // If the control might be a hotkey, check it
        if (control > Control.HotkeyOffset)
        {
            var hotkeyValue = control - Control.HotkeyOffset;
            var hotbarSlotCount = Options.Instance?.Player.HotbarSlotCount ?? PlayerOptions.DefaultHotbarSlotCount;

            if (hotkeyValue <= hotbarSlotCount)
            {
                // Even if the enum isn't defined, if it's within our configured
                // hotbar slot count it should be considered valid.
                return true;
            }

            // If the enum is defined, but the hotbar slot is above the slot count, it should be considered invalid
            if (Enum.IsDefined(control) && hotkeyValue > PlayerOptions.DefaultHotbarSlotCount)
            {
                return false;
            }
        }

        return true;
    }
}