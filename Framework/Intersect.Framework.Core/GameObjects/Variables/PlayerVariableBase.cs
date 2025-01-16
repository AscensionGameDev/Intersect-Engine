using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public partial class PlayerVariableBase : VariableDescriptor<PlayerVariableBase>, IVariableDescriptor
{
    [JsonConstructor]
    public PlayerVariableBase(Guid id) : base(id)
    {
        Name = "New Player Variable";
    }

    public PlayerVariableBase()
    {
        Name = "New Player Variable";
    }
}
