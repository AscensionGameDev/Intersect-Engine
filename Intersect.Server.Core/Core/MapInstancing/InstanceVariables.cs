using System.Collections.Concurrent;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Server.Core.MapInstancing;

/// <summary>
/// In-memory values of instance variables, keyed by map instance id then variable id. The overworld
/// (<see cref="Guid.Empty"/>) has its own bucket too, so the commands never fail outside an instance.
/// </summary>
public static class InstanceVariables
{
    private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, VariableValue>> Values = new();

    /// <summary>
    /// Returns the live value object for the variable on the instance, creating it (with the variable's default) on first use.
    /// </summary>
    public static VariableValue GetVariableValue(Guid mapInstanceId, Guid variableId)
    {
        var bucket = Values.GetOrAdd(mapInstanceId, _ => new ConcurrentDictionary<Guid, VariableValue>());

        return bucket.GetOrAdd(variableId, _ => CreateDefault(variableId));
    }

    /// <summary>
    /// A fresh value already carries the type declared in the editor. Without it the first comparison to touch the value
    /// would decide its type, and a stale condition of another type could make every other check fail.
    /// </summary>
    private static VariableValue CreateDefault(Guid variableId)
    {
        var value = new VariableValue();
        switch (InstanceVariableDescriptor.Get(variableId)?.DataType)
        {
            case VariableDataType.Boolean:
                value.Boolean = false;
                break;
            case VariableDataType.Integer:
                value.Integer = 0;
                break;
            case VariableDataType.Number:
                value.Number = 0;
                break;
            case VariableDataType.String:
                value.String = string.Empty;
                break;
        }

        return value;
    }

    /// <summary>
    /// Forgets every value of an instance (call when the instance is torn down).
    /// </summary>
    public static void Clear(Guid mapInstanceId)
    {
        if (mapInstanceId == Guid.Empty)
        {
            return;
        }

        Values.TryRemove(mapInstanceId, out _);
    }
}
