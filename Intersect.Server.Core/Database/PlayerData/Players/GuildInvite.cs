using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Server.Entities;

namespace Intersect.Server.Database.PlayerData.Players;

public record struct GuildInvite
{
    [ForeignKey(nameof(FromId))]
    public Player? From { get; init; }

    public Guid FromId { get; init; }

    [ForeignKey(nameof(ToId))]
    public Guild? To { get; init; }

    public Guid ToId { get; init; }

    [NotMapped]
    public bool IsValid => ToId != default || To != null;
}