using Intersect.Enums;
using Intersect.Models;

namespace Intersect.GameObjects.Switches_and_Variables;
public interface IVariableBase : IDatabaseObject, IFolderable
{
    new VariableDataType Type { get; set; }

    string TextId { get; set; }
}
