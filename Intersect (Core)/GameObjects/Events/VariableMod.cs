using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Enums;

namespace Intersect.GameObjects.Events
{
    public class VariableMod
    {

    }

    public class IntegerVariableMod : VariableMod
    {
        public VariableMods ModType { get; set; } = VariableMods.Set;
        public int Value { get; set; }
        public int HighValue { get; set; }
        public Guid DupVariableId { get; set; }
    }

    public class BooleanVariableMod : VariableMod
    {
        public bool Value { get; set; }
        public VariableTypes DupVariableType { get; set; } = VariableTypes.PlayerVariable;
        public Guid DupVariableId { get; set; }
    }
}
