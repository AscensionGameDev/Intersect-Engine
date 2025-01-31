using System.Diagnostics.CodeAnalysis;

namespace Intersect.Client.Framework.Input;

public interface IControlsProvider
{
    Control[] Controls { get; }

    bool TryGetDefaultMapping(Control control, [NotNullWhen(true)] out ControlMapping? defaultMapping);

    void ReloadFromOptions(Options? options);
}