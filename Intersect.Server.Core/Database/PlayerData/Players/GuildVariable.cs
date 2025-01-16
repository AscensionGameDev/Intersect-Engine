using Intersect.GameObjects;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Server.Database.PlayerData.Players;

public partial class GuildVariable : Variable
{
    public GuildVariable() : this(Guid.Empty) { }

    public GuildVariable(Guid id)
    {
        VariableId = id;
    }

    [NotMapped]
    public string VariableName => GuildVariableDescriptor.GetName(VariableId);

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonIgnore]
    public Guid GuildId { get; protected set; }

    [JsonIgnore]
    [ForeignKey(nameof(GuildId))]
    public virtual Guild Guild { get; protected set; }
}
