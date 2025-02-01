using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Intersect.Client.Framework.Input;

public partial class ControlMapping
{
    public List<ControlBinding> Bindings { get; }

    [JsonConstructor]
    public ControlMapping(ControlBinding binding, params ControlBinding[] alternateBindings)
    {
        Bindings = new List<ControlBinding>(1 + (alternateBindings?.Length ?? 0)) { binding };
        Bindings.AddRange(alternateBindings ?? []);

        if (Bindings.Count < 2)
        {
            Bindings.Add(ControlBinding.Default);
        }
    }

    public ControlMapping(ControlMapping controlMapping)
    {
        ArgumentNullException.ThrowIfNull(controlMapping, nameof(controlMapping));

        Bindings = controlMapping.Bindings.Select(binding => new ControlBinding(binding)).ToList();
    }

    public bool IsActive([NotNullWhen(true)] out ControlBinding? activeBinding)
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var binding in Bindings)
        {
            if (!binding.IsDown())
            {
                continue;
            }

            if (!binding.IsMouseKey)
            {
                activeBinding = binding;
                return true;
            }

            if (GameInput.Current.MouseHitInterface)
            {
                continue;
            }

            activeBinding = binding;
            return true;
        }

        activeBinding = null;
        return false;
    }
}
