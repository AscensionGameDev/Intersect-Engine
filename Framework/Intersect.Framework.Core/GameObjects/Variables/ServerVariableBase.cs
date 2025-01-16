using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Variables;

public partial class ServerVariableBase : VariableDescriptor<ServerVariableBase>, IVariableDescriptor
{
    [JsonConstructor]
    public ServerVariableBase(Guid id) : base(id)
    {
        Name = "New Global Variable";
    }

    public ServerVariableBase()
    {
        Name = "New Global Variable";
    }

    [NotMapped]
    [JsonIgnore]
    public VariableValue Value { get; set; } = new();

    [NotMapped]
    [JsonProperty(nameof(Value))]
    public dynamic ValueData
    {
        get => Value.Value;
        set => Value.Value = value;
    }

    [Column(nameof(Value))]
    [JsonIgnore]
    public string Json
    {
        get => Value.Json.ToString(Formatting.None);
        private set
        {
            if (VariableValue.TryParse(value, out var json))
            {
                Value.Json = json;
            }
        }
    }
}
