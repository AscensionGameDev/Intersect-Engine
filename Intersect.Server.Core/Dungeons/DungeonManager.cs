using System.Collections.Concurrent;

namespace Intersect.Server.Dungeons;

/// <summary>
/// Registry of the dungeon runs currently alive on the server.
/// </summary>
public static class DungeonManager
{
    private const long TickIntervalMs = 500;

    private static readonly ConcurrentDictionary<Guid, DungeonInstance> Instances = new();

    private static long _nextTick;

    public static void Add(DungeonInstance instance) => Instances[instance.MapInstanceId] = instance;

    public static void Remove(DungeonInstance instance)
    {
        Instances.TryRemove(instance.MapInstanceId, out _);
        Core.MapInstancing.InstanceVariables.Clear(instance.MapInstanceId);
    }

    public static bool TryGet(Guid mapInstanceId, out DungeonInstance instance) => Instances.TryGetValue(mapInstanceId, out instance!);

    private static readonly ConcurrentDictionary<Guid, DungeonInvite> Invites = new();

    /// <summary>Pending party invitations, keyed by leader id (one open invite per leader).</summary>
    public static bool TryGetInvite(Guid leaderId, out DungeonInvite invite) => Invites.TryGetValue(leaderId, out invite!);

    public static void AddInvite(DungeonInvite invite) => Invites[invite.Leader.Id] = invite;

    public static void RemoveInvite(DungeonInvite invite) => Invites.TryRemove(invite.Leader.Id, out _);

    /// <summary>
    /// Cheap to call often (every player update): the real work only runs every <see cref="TickIntervalMs"/>.
    /// </summary>
    public static void Update(long timeMs)
    {
        if (timeMs < _nextTick)
        {
            return;
        }

        _nextTick = timeMs + TickIntervalMs;

        foreach (var instance in Instances.Values)
        {
            instance.Update(timeMs);
        }

        foreach (var invite in Invites.Values)
        {
            invite.Update(timeMs);
        }
    }
}
