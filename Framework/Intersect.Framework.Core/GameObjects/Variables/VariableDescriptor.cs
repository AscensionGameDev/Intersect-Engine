using Intersect.Enums;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public abstract class VariableDescriptor<TObject> : DatabaseObject<TObject> where TObject : VariableDescriptor<TObject>, IVariableDescriptor
{
    protected VariableDescriptor()
    {
    }

    [JsonConstructor]
    protected VariableDescriptor(Guid descriptorId) : base(descriptorId)
    {
    }

    /// <inheritdoc cref="IVariableDescriptor.DataType" />
    public VariableDataType DataType { get; set; } = VariableDataType.Boolean;

    /// <inheritdoc cref="IFolderable.Folder"/>
    public string? Folder { get; set; }

    /// <inheritdoc cref="IVariableDescriptor.TextId" />
    public string TextId { get; set; }

    /// <summary>
    /// Retrieve an array of variable names of the supplied data type.
    /// </summary>
    /// <param name="dataType">The data type to retrieve names of.</param>
    /// <returns>Returns an array of names.</returns>
    public static string[] GetNamesByType(VariableDataType dataType) =>
        Lookup
            .Where(pair => pair.Value is IVariableDescriptor descriptor && descriptor.DataType == dataType)
            .OrderBy(pair => pair.Value.TimeCreated)
            .Select(pair => pair.Value.Name)
            .ToArray();

    /// <summary>
    /// Retrieve the list index of an ID within a specific data type list.
    /// </summary>
    /// <param name="id">The ID to look up.</param>
    /// <param name="dataType">The data type to search up.</param>
    /// <returns>Returns the list Index of the provided ID.</returns>
    public static int ListIndex(Guid id, VariableDataType dataType) =>
        Lookup
            .Where(pair => pair.Value is IVariableDescriptor descriptor && descriptor.DataType == dataType)
            .OrderBy(pair => pair.Value.TimeCreated)
            .ToList()
            .FindIndex(pair => pair.Value.Id == id);

    /// <summary>
    /// Retrieve the ID associated with a list index of a specific data type.
    /// </summary>
    /// <param name="listIndex">The list index to retrieve.</param>
    /// <param name="dataType">The data type to search up.</param>
    /// <returns>Returns the ID of the provided index.</returns>
    public static Guid IdFromList(int listIndex, VariableDataType dataType)
    {
        if (listIndex < 0 || listIndex > GetNamesByType(dataType).Length)
        {
            return Guid.Empty;
        }

        return Lookup
            .Where(pair => pair.Value is IVariableDescriptor descriptor && descriptor.DataType == dataType)
            .OrderBy(pair => pair.Value.TimeCreated)
            .Skip(listIndex)
            .First()
            .Value.Id;
    }
}
