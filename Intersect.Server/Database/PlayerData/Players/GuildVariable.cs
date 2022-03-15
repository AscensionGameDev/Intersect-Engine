using Intersect.GameObjects;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Database.PlayerData.Players
{
    public partial class GuildVariable : Variable
    {
        public GuildVariable() : base() { }

        public GuildVariable(Guid id) : base(id) { }

        [NotMapped]
        public string VariableName => GuildVariableBase.GetName(VariableId);

        [JsonIgnore]
        public Guid GuildId { get; protected set; }

        [JsonIgnore]
        public virtual Guild Guild { get; protected set; }
    }
}
