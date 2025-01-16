using Intersect.GameObjects;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.GameObjects.Variables;

namespace Intersect.Server.Database.PlayerData.Players;

public partial class UserVariable : Variable
{
    public UserVariable() : this(Guid.Empty) { }

    public UserVariable(Guid userVariableBaseId)
    {
        VariableId = userVariableBaseId;
    }

    [NotMapped]
    public string VariableName => UserVariableDescriptor.GetName(VariableId);

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonIgnore]
    public Guid UserId { get; protected set; }

    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; protected set; }
}
