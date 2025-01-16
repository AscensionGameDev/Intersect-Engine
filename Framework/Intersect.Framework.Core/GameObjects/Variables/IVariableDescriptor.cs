using Intersect.Enums;
using Intersect.Models;

namespace Intersect.Framework.Core.GameObjects.Variables;

public interface IVariableDescriptor : IDatabaseObject, IFolderable
{
    /// <summary>
    /// The data type of the variable.
    /// </summary>
    VariableDataType DataType { get; set; }

    /// <summary>
    /// Identifier used for event chat variables to display the value of this variable/switch.
    /// </summary>
    /// <seealso href="https://www.ascensiongamedev.com/topic/749-event-text-variables/" />
    string TextId { get; set; }
}
