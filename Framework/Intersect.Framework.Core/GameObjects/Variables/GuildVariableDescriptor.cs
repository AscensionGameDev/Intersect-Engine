using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public partial class GuildVariableDescriptor : VariableDescriptor<GuildVariableDescriptor>, IVariableDescriptor
{
    [JsonConstructor]
    public GuildVariableDescriptor(Guid id) : base(id)
    {
        Name = "New Guild Variable";
    }

    public GuildVariableDescriptor()
    {
        Name = "New Guild Variable";
    }
}
