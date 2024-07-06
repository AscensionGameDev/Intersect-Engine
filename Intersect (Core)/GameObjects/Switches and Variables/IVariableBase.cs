using Intersect.Enums;
using Intersect.Models;

namespace Intersect.GameObjects.Switches_and_Variables;
public interface IVariableBase : IDatabaseObject, IFolderable
{
    public VariableDataType Type { get; set; }

    public string TextId { get; set; }
}
