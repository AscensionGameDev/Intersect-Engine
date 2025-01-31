using System.Diagnostics.CodeAnalysis;

namespace Intersect.Client.Framework.Input;

public interface IControlSet
{
    ControlMapping? this[Control control] { get; set; }

    IReadOnlyDictionary<Control, ControlMapping> Mappings { get; }

    void ReloadFromOptions(Options options);

    void ResetDefaults();

    bool TryAdd(Control control, params ControlBinding[] bindings);

    bool TryAdd(Control control, ControlMapping mapping);

    bool TryGetMappingFor(Control control, [NotNullWhen(true)] out ControlMapping? mapping);

    bool TryLoad();

    bool TryReload();

    bool TrySave();
}