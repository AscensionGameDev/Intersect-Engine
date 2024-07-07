﻿using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Server.Entities;

using Newtonsoft.Json;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Intersect.Server.Database.PlayerData.Players;


public partial class Switch : IPlayerOwned
{

    public Switch()
    {
    }

    public Switch(Guid id)
    {
        SwitchId = id;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid SwitchId { get; private set; }

    public bool Value { get; set; }

    public Guid PlayerId { get; private set; }

    [JsonIgnore]
    public virtual Player Player { get; private set; }

}
