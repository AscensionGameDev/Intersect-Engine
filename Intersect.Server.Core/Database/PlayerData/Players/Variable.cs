using Intersect.GameObjects.Switches_and_Variables;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.PlayerData.Players;

public partial class Variable
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public Guid Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid VariableId { get; protected set; }

    [NotMapped]
    [JsonIgnore]
    public VariableValue Value { get; set; } = new();

    [NotMapped]
    [JsonProperty("Value")]
    public dynamic ValueData => Value.Value;

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
