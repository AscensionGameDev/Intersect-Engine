using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public partial class GuildVariableBase : VariableDescriptor<GuildVariableBase>, IVariableDescriptor
{
    [JsonConstructor]
    public GuildVariableBase(Guid id) : base(id)
    {
        Name = "New Guild Variable";
    }

    public GuildVariableBase()
    {
        Name = "New Guild Variable";
    }
}
