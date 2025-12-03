using System.Diagnostics.CodeAnalysis;
using Intersect.Config;

namespace Intersect.Client.Framework.Input;

internal sealed class HotkeyControlsProvider : IControlsProvider
{
    private Dictionary<Control, ControlMapping>? _defaultMappings = null;

    public HotkeyControlsProvider() => Controls = ControlsFromOptions(Options.Instance);

    public Control[] Controls { get; private set; }

    public bool TryGetDefaultMapping(Control control, [NotNullWhen(true)] out ControlMapping? defaultMapping)
    {
        _defaultMappings ??= BuiltinControlsProvider.LoadDefaultMappings(); 
        return _defaultMappings.TryGetValue(control, out defaultMapping);
    }

    private static Control[] ControlsFromOptions(Options? options) =>
        Enumerable
            .Range(1, options?.Player.HotbarSlotCount ?? PlayerOptions.DefaultHotbarSlotCount)
            .Select(hotkeyValue => Control.HotkeyOffset + hotkeyValue)
            .ToArray();

    public void ReloadFromOptions(Options? options)
    {
        Controls = ControlsFromOptions(options);
        _defaultMappings = null; // Force reload with new hotbar slot count
    }
}