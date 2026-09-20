using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

/// <summary>
/// A variable shared by everyone on the same map instance (dungeon run, shared/guild instance). Values live in memory
/// only: they start at their default whenever a new instance is created and vanish with it.
/// </summary>
public partial class InstanceVariableDescriptor : VariableDescriptor<InstanceVariableDescriptor>, IVariableDescriptor
{
    [JsonConstructor]
    public InstanceVariableDescriptor(Guid id) : base(id)
    {
        Name = "New Instance Variable";
    }

    public InstanceVariableDescriptor()
    {
        Name = "New Instance Variable";
    }
}
