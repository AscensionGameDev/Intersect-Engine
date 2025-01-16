using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public partial class UserVariableDescriptor : VariableDescriptor<UserVariableDescriptor>, IVariableDescriptor
{
    [JsonConstructor]
    public UserVariableDescriptor(Guid id) : base(id)
    {
        Name = "New User Variable";
    }

    public UserVariableDescriptor()
    {
        Name = "New User Variable";
    }
}
