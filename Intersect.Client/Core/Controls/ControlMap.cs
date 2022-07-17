using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Client.Core.Controls
{
    public partial class ControlMap
    {
        public List<ControlValue> Bindings { get; }

        public ControlMap(ControlValue binding, params ControlValue[] alternateBindings)
        {
            Bindings = new List<ControlValue>(1 + (alternateBindings?.Length ?? 0)) { binding };
            Bindings.AddRange(alternateBindings ?? Array.Empty<ControlValue>());

            if (Bindings.Count < 2)
            {
                Bindings.Add(ControlValue.Default);
            }
        }

        public ControlMap(ControlMap controlMap)
        {
            if (controlMap == default)
            {
                throw new ArgumentNullException(nameof(controlMap));
            }

            var bindings = controlMap.Bindings.ToList();
            if (bindings.Count < 1)
            {
                throw new ArgumentException("The control map does not have at least one binding.", nameof(controlMap));
            }

            Bindings = new List<ControlValue>(bindings);
        }

        public bool KeyDown() => Bindings.Any(button => button.IsDown() && (!button.IsMouseKey || !Interface.Interface.MouseHitGui()));
    }
}
