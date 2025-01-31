using System.Diagnostics.CodeAnalysis;

namespace Intersect.Client.Framework.Input;

internal sealed class BuiltinControlsProvider : IControlsProvider
{
    public Control[] Controls { get; } = Enum.GetValues<Control>().Where(control => control.IsValid()).ToArray();

    public bool TryGetDefaultMapping(Control control, [NotNullWhen(true)] out ControlMapping? defaultMapping)
    {
        defaultMapping = null;
        return false;
    }

    public void ReloadFromOptions(Options? options)
    {
    }
}