using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Server.Database;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Network.Packets.Server;
using Intersect.Server.Dungeons.Generation;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

namespace Intersect.Server.Dungeons;

/// <summary>
/// One live run of a dungeon: the map instance every participant shares, the shared life pool, the timer,
/// the objective line and the end-of-run bookkeeping (rewards, records, sending everyone back out).
/// </summary>
public sealed class DungeonInstance
{
    private readonly object _lock = new();

    private readonly HashSet<Player> _players = new();

    public DungeonInstance(DungeonDescriptor descriptor, Player starter)
    {
        Descriptor = descriptor;
        MapInstanceId = Guid.NewGuid();
        StartTime = Timing.Global.Milliseconds;
        Lives = descriptor.Lives;
        Objective = string.Empty;

        if (descriptor.ProceduralGeneration)
        {
            GeneratedMap = DungeonMapGenerator.Generate(descriptor, starter, MapInstanceId, out var entryX, out var entryY);
            GeneratedEntryX = entryX;
            GeneratedEntryY = entryY;
        }
    }

    public DungeonDescriptor Descriptor { get; }

    /// <summary>The map instance id all participants are warped onto.</summary>
    public Guid MapInstanceId { get; }

    /// <summary>The map generated for this run, or <c>null</c> when <see cref="DungeonDescriptor.ProceduralGeneration"/> is off.</summary>
    public MapController? GeneratedMap { get; }

    public int GeneratedEntryX { get; }

    public int GeneratedEntryY { get; }

    /// <summary>The map/coordinates participants should warp/respawn onto for this run.</summary>
    public (Guid MapId, int X, int Y, Direction Direction) EntryPoint => GeneratedMap != null
        ? (GeneratedMap.Id, GeneratedEntryX, GeneratedEntryY, Direction.Down)
        : (Descriptor.EntryMapId, Descriptor.EntryX, Descriptor.EntryY, Descriptor.EntryDirection);

    public long StartTime { get; }

    /// <summary>When the run ended (completed or failed), 0 while running.</summary>
    public long EndTime { get; private set; }

    public DungeonRunState State { get; private set; } = DungeonRunState.Running;

    public int Lives { get; private set; }

    public string Objective { get; private set; }

    public bool IsRunning => State == DungeonRunState.Running;

    public long ElapsedMs => (EndTime == 0 ? Timing.Global.Milliseconds : EndTime) - StartTime;

    public long RemainingMs => Descriptor.TimeLimitSeconds <= 0
        ? -1
        : Math.Max(0, StartTime + Descriptor.TimeLimitSeconds * 1000L - Timing.Global.Milliseconds);

    public Player[] Players
    {
        get
        {
            lock (_lock)
            {
                return _players.ToArray();
            }
        }
    }

    public int PlayerCount
    {
        get
        {
            lock (_lock)
            {
                return _players.Count;
            }
        }
    }

    public void AddPlayer(Player player)
    {
        lock (_lock)
        {
            _players.Add(player);
        }

        player.CurrentDungeon = this;
        SendStatus(player);
    }

    /// <summary>
    /// Detaches a player from the run (logout, abandon, kicked out at the end). Does not warp them.
    /// </summary>
    public void RemovePlayer(Player player)
    {
        lock (_lock)
        {
            _players.Remove(player);
        }

        if (player.CurrentDungeon == this)
        {
            player.CurrentDungeon = null;
        }

        PacketSender.SendDungeonStatus(player);

        if (PlayerCount == 0)
        {
            DungeonManager.Remove(this);
        }
    }

    /// <summary>
    /// Called once per tick by <see cref="DungeonManager"/>.
    /// </summary>
    public void Update(long timeMs)
    {
        if (IsRunning)
        {
            if (Descriptor.TimeLimitSeconds > 0 && RemainingMs <= 0)
            {
                Fail(Strings.Dungeons.OutOfTime);
            }

            return;
        }

        // Run is over: give people a moment to read the result, then send them home.
        if (timeMs >= EndTime + Descriptor.ExitDelaySeconds * 1000L)
        {
            foreach (var player in Players)
            {
                player.LeaveDungeon(sendHome: true);
            }
        }
    }

    public void SetObjective(string objective)
    {
        Objective = objective ?? string.Empty;
        BroadcastStatus();
    }

    /// <summary>
    /// A participant died. Spends a shared life (if the dungeon has lives) and reports whether the run can go on.
    /// </summary>
    public bool OnPlayerDied(Player player)
    {
        if (!IsRunning)
        {
            return false;
        }

        if (Descriptor.Lives <= 0)
        {
            return true;
        }

        Lives = Math.Max(0, Lives - 1);
        if (Lives > 0)
        {
            Broadcast(Strings.Dungeons.LivesRemaining.ToString(player.Name, Lives));
            BroadcastStatus();

            return true;
        }

        Fail(Strings.Dungeons.OutOfLives);

        return false;
    }

    public void Complete()
    {
        if (!IsRunning)
        {
            return;
        }

        State = DungeonRunState.Completed;
        EndTime = Timing.Global.Milliseconds;

        var elapsed = ElapsedMs;
        foreach (var player in Players)
        {
            var previousBest = player.DungeonData.Records.TryGetValue(Descriptor.Id, out var record) ? record.BestTimeMs : 0;
            GiveRewards(player);
            player.RecordDungeonCompletion(Descriptor.Id, elapsed);
            DbInterface.UpsertDungeonLeaderboardEntry(Descriptor.Id, player.Name, elapsed);
            player.SendPacket(BuildResultPacket(completed: true, string.Empty, previousBest));
            PacketSender.SendChatMsg(
                player,
                Strings.Dungeons.Completed.ToString(Descriptor.Name, FormatTime(elapsed)),
                ChatMessageType.Notice,
                CustomColors.Alerts.Accepted
            );
        }

        // The run is over: whatever is still alive in the dungeon (an unkilled boss included) disappears,
        // otherwise it could still be attacked until the players leave the instance.
        DespawnRemainingNpcs();

        BroadcastStatus();
    }

    private void DespawnRemainingNpcs()
    {
        foreach (var controller in MapController.Lookup.Values.OfType<MapController>().ToArray())
        {
            if (!controller.TryGetInstance(MapInstanceId, out var instance) || instance == default)
            {
                continue;
            }

            foreach (var npc in instance.GetCachedEntities().OfType<Npc>().ToArray())
            {
                // Keep companions that belong to players.
                if (npc.Owner is Player)
                {
                    continue;
                }

                npc.Die(false);
            }
        }
    }

    public void Fail(string reason)
    {
        if (!IsRunning)
        {
            return;
        }

        State = DungeonRunState.Failed;
        EndTime = Timing.Global.Milliseconds;
        Broadcast(Strings.Dungeons.Failed.ToString(Descriptor.Name, reason), CustomColors.Alerts.Declined);
        foreach (var player in Players)
        {
            player.SendPacket(BuildResultPacket(completed: false, reason, 0));
        }

        BroadcastStatus();
    }

    private DungeonResultPacket BuildResultPacket(bool completed, string failReason, long previousBestMs) => new()
    {
        DungeonId = Descriptor.Id,
        Completed = completed,
        FailReason = failReason,
        ElapsedMs = ElapsedMs,
        PreviousBestMs = previousBestMs,
        Rewards = completed
            ? Descriptor.Rewards
                .Where(reward => reward.ItemId != Guid.Empty && reward.Quantity > 0 && ItemDescriptor.Get(reward.ItemId) != null)
                .Select(reward => new DungeonRewardPacket(reward.ItemId, reward.Quantity))
                .ToArray()
            : [],
        RewardExperience = completed ? Descriptor.RewardExperience : 0,
        RewardTitleId = completed ? Descriptor.RewardTitleId : Guid.Empty,
        ExitDelaySeconds = Descriptor.ExitDelaySeconds,
    };

    public DungeonStatusPacket BuildStatusPacket() => new(
        Descriptor.Id,
        State,
        ElapsedMs,
        RemainingMs,
        Lives,
        Descriptor.Lives,
        Objective
    );

    public void BroadcastStatus()
    {
        foreach (var player in Players)
        {
            SendStatus(player);
        }
    }

    private void SendStatus(Player player) => player.SendPacket(BuildStatusPacket());

    private void Broadcast(string message, Color? color = null)
    {
        foreach (var player in Players)
        {
            PacketSender.SendChatMsg(player, message, ChatMessageType.Notice, color ?? CustomColors.Alerts.Info);
        }
    }

    private void GiveRewards(Player player)
    {
        foreach (var reward in Descriptor.Rewards)
        {
            if (reward.ItemId == Guid.Empty || reward.Quantity <= 0 || ItemDescriptor.Get(reward.ItemId) == null)
            {
                continue;
            }

            // Overflow goes to the ground/bank so nobody loses their loot to a full bag.
            player.TryGiveItem(reward.ItemId, reward.Quantity, ItemHandling.Overflow, true);
        }

        if (Descriptor.RewardExperience > 0)
        {
            player.GiveExperience(Descriptor.RewardExperience);
        }

        if (Descriptor.RewardTitleId != Guid.Empty)
        {
            player.UnlockTitle(Descriptor.RewardTitleId);
        }

        if (Descriptor.RewardAchievementId != Guid.Empty)
        {
            player.UnlockAchievement(Descriptor.RewardAchievementId);
        }
    }

    public static string FormatTime(long ms)
    {
        var time = TimeSpan.FromMilliseconds(Math.Max(0, ms));

        return time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
    }
}
