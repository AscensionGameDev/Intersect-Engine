using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public partial class PlayerVariableDescriptor : VariableDescriptor<PlayerVariableDescriptor>, IVariableDescriptor
{
    [JsonConstructor]
    public PlayerVariableDescriptor(Guid id) : base(id)
    {
        Name = "New Player Variable";
    }

    public PlayerVariableDescriptor()
    {
        Name = "New Player Variable";
    }
}
