using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public partial class UserVariableBase : VariableDescriptor<UserVariableBase>, IVariableDescriptor
{
    [JsonConstructor]
    public UserVariableBase(Guid id) : base(id)
    {
        Name = "New User Variable";
    }

    public UserVariableBase()
    {
        Name = "New User Variable";
    }
}
