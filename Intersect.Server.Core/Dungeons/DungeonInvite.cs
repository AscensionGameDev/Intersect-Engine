using Intersect.Enums;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Dungeons;
using Intersect.Network.Packets.Server;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Dungeons;

/// <summary>
/// A party dungeon about to start: the leader has asked, members have a few seconds to accept.
/// Resolved (run started with everyone who accepted, or cancelled) when everyone answered or the timer runs out.
/// </summary>
public sealed class DungeonInvite
{
    public const long TimeoutMs = 20_000;

    private readonly object _lock = new();

    private readonly HashSet<Player> _pending = new();

    private readonly List<Player> _accepted = new();

    public DungeonInvite(DungeonDescriptor descriptor, Player leader, IEnumerable<Player> members, byte difficulty = 0)
    {
        Descriptor = descriptor;
        Difficulty = difficulty;
        Leader = leader;
        ExpiresAt = Timing.Global.Milliseconds + TimeoutMs;
        _accepted.Add(leader);

        foreach (var member in members)
        {
            if (member != leader)
            {
                _pending.Add(member);
            }
        }
    }

    public DungeonDescriptor Descriptor { get; }

    public byte Difficulty { get; }

    public Player Leader { get; }

    public long ExpiresAt { get; }

    public bool IsResolved { get; private set; }

    public void Send()
    {
        var packet = new DungeonInvitePacket(Descriptor.Id, Leader.Id, Leader.Name, TimeoutMs);
        Player[] pending;
        lock (_lock)
        {
            pending = _pending.ToArray();
        }

        foreach (var member in pending)
        {
            member.PendingDungeonInvite = this;
            member.SendPacket(packet);
        }

        PacketSender.SendChatMsg(Leader, Strings.Dungeons.InviteSent.ToString(pending.Length), ChatMessageType.Notice, CustomColors.Alerts.Info);
    }

    public void Respond(Player member, bool accepted)
    {
        lock (_lock)
        {
            if (!_pending.Remove(member))
            {
                return;
            }

            if (accepted)
            {
                _accepted.Add(member);
            }
        }

        member.PendingDungeonInvite = null;
        PacketSender.SendChatMsg(
            Leader,
            accepted ? Strings.Dungeons.InviteAccepted.ToString(member.Name) : Strings.Dungeons.InviteDeclined.ToString(member.Name),
            ChatMessageType.Notice,
            accepted ? CustomColors.Alerts.Accepted : CustomColors.Alerts.Declined
        );

        if (_pending.Count == 0)
        {
            Resolve();
        }
    }

    /// <summary>
    /// Called by <see cref="DungeonManager"/>: resolves the invite once it expired.
    /// </summary>
    public void Update(long timeMs)
    {
        if (!IsResolved && timeMs >= ExpiresAt)
        {
            Resolve();
        }
    }

    public void Cancel()
    {
        if (IsResolved)
        {
            return;
        }

        IsResolved = true;
        ClearPending();
        DungeonManager.RemoveInvite(this);
    }

    private void Resolve()
    {
        if (IsResolved)
        {
            return;
        }

        IsResolved = true;
        ClearPending();
        DungeonManager.RemoveInvite(this);

        Player[] participants;
        lock (_lock)
        {
            participants = _accepted.Where(player => !player.IsDisposed).ToArray();
        }

        if (participants.Length < Math.Max(1, Descriptor.MinPlayers))
        {
            foreach (var player in participants)
            {
                PacketSender.SendChatMsg(player, Strings.Dungeons.NotEnoughPlayers.ToString(Descriptor.MinPlayers), ChatMessageType.Notice, CustomColors.Alerts.Declined);
            }

            return;
        }

        Player.StartDungeonRun(Descriptor, Leader, participants, Difficulty);
    }

    private void ClearPending()
    {
        Player[] pending;
        lock (_lock)
        {
            pending = _pending.ToArray();
            _pending.Clear();
        }

        foreach (var member in pending)
        {
            if (member.PendingDungeonInvite == this)
            {
                member.PendingDungeonInvite = null;
            }
        }
    }
}
