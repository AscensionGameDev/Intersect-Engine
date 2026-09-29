using Intersect.Enums;
using Intersect.Server.Localization;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Factions;
using Intersect.Framework.Core.GameObjects.Territories;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Networking;

namespace Intersect.Server.Territories;

/// <summary>
/// Ticks every <see cref="TerritoryDescriptor"/>: finds who is standing near its capture point, advances or
/// freezes capture progress, flips ownership and pays out the controlling faction's periodic reward.
/// Live progress lives only in memory (reset on restart); who currently owns each territory is persisted via
/// <see cref="DbInterface.SetTerritoryControl"/> so restarts don't reset ownership.
/// </summary>
public static class TerritoryManager
{
    private const long TickIntervalMs = 1000;

    private static long _nextTick;

    private static bool _loaded;

    private sealed class RuntimeState
    {
        public Guid ControllingFactionId = Guid.Empty;

        public Guid ProgressFactionId = Guid.Empty;

        public double ProgressSeconds;

        public bool Contested;

        public long LastRewardTick;
    }

    private static readonly Dictionary<Guid, RuntimeState> States = new();

    /// <summary>Cheap to call often: the real work only runs every <see cref="TickIntervalMs"/>.</summary>
    public static void Update(long timeMs)
    {
        if (timeMs < _nextTick)
        {
            return;
        }

        _nextTick = timeMs + TickIntervalMs;

        if (!_loaded)
        {
            LoadControl();
            _loaded = true;
        }

        var elapsedSeconds = TickIntervalMs / 1000.0;
        foreach (var territory in TerritoryDescriptor.Lookup.Values.OfType<TerritoryDescriptor>())
        {
            UpdateTerritory(territory, elapsedSeconds, timeMs);
        }
    }

    private static void LoadControl()
    {
        var persisted = DbInterface.GetAllTerritoryControl();
        foreach (var territory in TerritoryDescriptor.Lookup.Values.OfType<TerritoryDescriptor>())
        {
            var state = new RuntimeState();
            if (persisted.TryGetValue(territory.Id, out var control))
            {
                state.ControllingFactionId = control.ControllingFactionId;
            }

            States[territory.Id] = state;
            BroadcastStatus(territory.Id, state);
        }
    }

    private static void UpdateTerritory(TerritoryDescriptor territory, double elapsedSeconds, long timeMs)
    {
        if (!States.TryGetValue(territory.Id, out var state))
        {
            state = new RuntimeState();
            States[territory.Id] = state;
        }

        var radiusSquared = territory.Radius * territory.Radius;
        var presentFactions = new HashSet<Guid>();
        var presentByFaction = new Dictionary<Guid, List<Player>>();

        foreach (var player in Player.OnlinePlayers)
        {
            if (player.FactionId == Guid.Empty || player.MapId != territory.MapId)
            {
                continue;
            }

            var dx = player.X - territory.X;
            var dy = player.Y - territory.Y;
            if (dx * dx + dy * dy > radiusSquared)
            {
                continue;
            }

            presentFactions.Add(player.FactionId);
            if (!presentByFaction.TryGetValue(player.FactionId, out var list))
            {
                presentByFaction[player.FactionId] = list = [];
            }

            list.Add(player);
        }

        var changed = false;
        state.Contested = presentFactions.Count > 1;

        if (presentFactions.Count == 1)
        {
            var soleFaction = presentFactions.First();
            if (soleFaction != state.ControllingFactionId)
            {
                if (state.ProgressFactionId != soleFaction)
                {
                    state.ProgressFactionId = soleFaction;
                    state.ProgressSeconds = 0;
                }

                state.ProgressSeconds += elapsedSeconds;
                changed = true;

                if (state.ProgressSeconds >= territory.CaptureTimeSeconds)
                {
                    Capture(territory, state, soleFaction);
                }
            }
            else if (state.ProgressFactionId != Guid.Empty)
            {
                // The defenders showed back up before anyone else finished capturing it.
                state.ProgressFactionId = Guid.Empty;
                state.ProgressSeconds = 0;
                changed = true;
            }
        }

        // Periodic reward for the controlling faction's members currently defending the point.
        if (state.ControllingFactionId != Guid.Empty &&
            territory.RewardTickSeconds > 0 &&
            timeMs - state.LastRewardTick >= territory.RewardTickSeconds * 1000L)
        {
            state.LastRewardTick = timeMs;
            if (territory.RewardExperience > 0 && presentByFaction.TryGetValue(state.ControllingFactionId, out var defenders))
            {
                foreach (var defender in defenders)
                {
                    defender.GiveExperience(territory.RewardExperience);
                }
            }
        }

        if (changed)
        {
            BroadcastStatus(territory.Id, state);
        }
    }

    private static void Capture(TerritoryDescriptor territory, RuntimeState state, Guid factionId)
    {
        state.ControllingFactionId = factionId;
        state.ProgressFactionId = Guid.Empty;
        state.ProgressSeconds = 0;
        state.LastRewardTick = 0;

        DbInterface.SetTerritoryControl(territory.Id, factionId);

        var factionName = FactionDescriptor.Get(factionId)?.Name ?? "Unknown";
        PacketSender.SendGlobalMsg(Strings.Territories.Captured.ToString(factionName, territory.Name));

        BroadcastStatus(territory.Id, state);
    }

    private static void BroadcastStatus(Guid territoryId, RuntimeState state)
    {
        PacketSender.SendDataToAllPlayers(
            new TerritoryStatusPacket(
                territoryId,
                state.ControllingFactionId,
                state.ProgressFactionId,
                (int)state.ProgressSeconds,
                state.Contested
            )
        );
    }
}
