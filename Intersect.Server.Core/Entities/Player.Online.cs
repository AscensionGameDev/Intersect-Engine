using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intersect.Server.Entities;

public partial class Player
{
    private static readonly ConcurrentDictionary<Guid, Player> OnlinePlayersById = [];
    // ReSharper disable once InconsistentNaming
    private static readonly HashSet<Player> _onlinePlayers = [];

    public static IReadOnlySet<Player> OnlinePlayers => _onlinePlayers;

    public static int OnlineCount => OnlinePlayersById.Count;

    [NotMapped] public bool IsOnline => OnlinePlayersById.ContainsKey(Id);
}