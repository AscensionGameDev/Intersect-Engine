using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects;
using Intersect.GameObjects.Switches_and_Variables;
using Intersect.Server.Entities;

using Newtonsoft.Json;

namespace Intersect.Server.Database.PlayerData.Players
{

    public partial class PlayerVariable : Variable, IPlayerOwned
    {
        public PlayerVariable() : base() { }

        public PlayerVariable(Guid id) : base(id) { }

        [NotMapped]
        public string VariableName => PlayerVariableBase.GetName(VariableId);

        [JsonIgnore]
        public Guid PlayerId { get; protected set; }

        [JsonIgnore]
        public virtual Player Player { get; protected set; }

    }

}
