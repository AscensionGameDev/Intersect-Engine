using System.Diagnostics.CodeAnalysis;
using Intersect.Config;

namespace Intersect.Client.Framework.Input;

internal sealed class HotkeyControlsProvider : IControlsProvider
{
    public HotkeyControlsProvider() => Controls = ControlsFromOptions(Options.Instance);

    public Control[] Controls { get; private set; }

    public bool TryGetDefaultMapping(Control control, [NotNullWhen(true)] out ControlMapping? defaultMapping)
    {
        defaultMapping = null;
        return false;
    }

    private static Control[] ControlsFromOptions(Options? options) =>
        Enumerable
            .Range(1, options?.Player.HotbarSlotCount ?? PlayerOptions.DefaultHotbarSlotCount)
            .Select(hotkeyValue => Control.HotkeyOffset + hotkeyValue)
            .ToArray();

    public void ReloadFromOptions(Options? options) => Controls = ControlsFromOptions(options);
}