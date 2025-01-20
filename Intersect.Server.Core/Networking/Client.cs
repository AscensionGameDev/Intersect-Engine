using System.Collections.Concurrent;
using Intersect.ErrorHandling;
using Intersect.Core;
using Intersect.Network;
using Intersect.Network.Packets;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Metrics;
using Intersect.Utilities;
using Microsoft.Extensions.Logging;
using Strings = Intersect.Server.Localization.Strings;

namespace Intersect.Server.Networking;


public partial class Client : IPacketSender
{

    public Guid EditorMap = Guid.Empty;

    private bool _crashing;

    //Client Properties
    public bool IsEditor;

    //Network Variables
    public IConnection Connection { get; private set; }

    private long mConnectionTimeout;

    private long mConnectTime;

    private bool mDebugPackets = false;

    private int mPacketCount = 0;

    private ConcurrentQueue<Tuple<IPacket, TransmissionMode, long>> mSendPacketQueue = new ConcurrentQueue<Tuple<IPacket, TransmissionMode, long>>();
    public ConcurrentQueue<IPacket> HandlePacketQueue = new ConcurrentQueue<IPacket>();
    public ConcurrentQueue<IPacket> RecentPackets = new ConcurrentQueue<IPacket>();
    public bool PacketHandlingQueued = false;
    public bool PacketSendingQueued = false;
    public Config.FloodThreshholds PacketFloodingThreshholds { get; set; } = Options.Instance.SecurityOpts?.PacketOpts.Threshholds;
    public long LastPing { get; set; } = -1;

    protected long mTimeout = 20000; //20 seconds

    private bool mBanChecked;

    //Sent Maps
    public Dictionary<Guid, Tuple<long, int>> SentMaps = new Dictionary<Guid, Tuple<long, int>>();

    private Client(IApplicationContext applicationContext, INetwork network, IConnection connection = null)
    {
        ApplicationContext = applicationContext;
        Network = network;

        Connection = connection;
        mConnectTime = Timing.Global.Milliseconds;
        mConnectionTimeout = Timing.Global.Milliseconds + mTimeout;

        PacketSender.SendServerConfig(this);
    }

    //Game Incorperation Variables
    public string Name => User?.Name;

    public string Email => User?.Email;

    public Guid Id => User?.Id ?? Guid.Empty;

    public string Password => User?.Password;

    public string Salt => User?.Salt;

    public bool Banned { get; set; }

    //Security/Flooding Variables
    public long AccountAttempts { get; set; }

    public long Ping => Connection.Statistics.Ping;

    /// <summary>
    /// Number of "grace" packets that the client has remaining if speedhacking is accidentally detected.
    /// </summary>
    public int TimedBufferPacketsRemaining { get; set; }

    public long TimeoutMs { get; set; }

    public long PacketTimer { get; set; }

    public bool PacketFloodDetect { get; set; }

    public long PacketCount { get; set; }

    public long FloodDetects { get; set; }

    public bool FloodKicked { get; set; }

    public long TotalFloodDetects { get; set; }

    public long LastPacketDesyncForgiven { get; set; }

    public UserRights Power
    {
        get => User?.Power ?? UserRights.None;
        set
        {
            if (User == null)
            {
                return;
            }

            User.Power = value;
        }
    }

    public User? User { get; private set; }

    public List<Player> Characters => User?.Players;

    public Player Entity { get; set; }

    public IApplicationContext ApplicationContext { get; }

    public INetwork Network { get; }

    public void SetUser(User user)
    {
        if (user == null)
        {
            User?.TryLogout();
        }

        if (user != null && user != User)
        {
            User.Login(user, Connection?.Ip);
        }

        User = user;
    }

    public void LoadCharacter(Player character)
    {
        //Entity = new Player(Id, this, character);
        Entity = character;

        if (Entity == null)
        {
            return;
        }

        Entity.LastOnline = DateTime.Now;
        Entity.Client = this;
    }


    public void Pinged()
    {
        if (Connection != null)
        {
            mConnectionTimeout = Timing.Global.Milliseconds + mTimeout;
        }
    }

    public void Disconnect(
        string? reason = default,
        bool shutdown = false,
        bool loggingOut = false,
        TaskCompletionSource? logoutCompletionSource = null
    )
    {
        lock (Globals.ClientLock)
        {
            if (Connection == null)
            {
                logoutCompletionSource?.TrySetResult();
                return;
            }

            if (!loggingOut)
            {
                Logout(force: shutdown, logoutCompletionSource: logoutCompletionSource);
            }
            else
            {
                logoutCompletionSource?.TrySetResult();
            }

            Globals.Clients.Remove(this);

            if (Connection != default)
            {
                Globals.ClientLookup.Remove(Connection.Guid);

                if (!Connection.CanDisconnect)
                {
                    return;
                }
            }

            Connection?.Disconnect(reason);
            Connection?.Dispose();
            Connection = default;
        }
    }

    public bool IsConnected()
    {
        return Connection?.IsConnected ?? false;
    }

    public string? Ip
    {
        get
        {
            try
            {
                return Connection?.Ip;
            }
            catch (Exception exception)
            {
                ApplicationContext.Logger.LogWarning(exception, $"Failed to get IP for user {User.Id}");
                return default;
            }
        }
    }

    public static Client CreateBeta4Client(IApplicationContext context, INetwork network, IConnection connection)
    {
        var client = new Client(context, network, connection);
        lock (Globals.ClientLock)
        {
            Globals.Clients.Add(client);
            Globals.ClientLookup.Add(connection.Guid, client);
        }

        return client;
    }

    public void Logout(bool force = false, TaskCompletionSource? logoutCompletionSource = null)
    {
        if (Entity is { } entity)
        {
            entity.TryLogout(logoutCompletionSource: logoutCompletionSource);
            Entity = null;
        }
        else
        {
            logoutCompletionSource?.TrySetResult();
        }

        if (User is { LoginTime: not null })
        {
            User.PlayTimeSeconds += (ulong)(DateTime.UtcNow - (DateTime)User.LoginTime).TotalSeconds;
            User.LoginTime = null;
        }

        if (!force && !IsEditor)
        {
            if (User?.Save() == UserSaveResult.DatabaseFailure)
            {
                LogAndDisconnect(Entity?.Id ?? default, nameof(Logout));
                return;
            }
        }

        SetUser(null);

        Disconnect("logout", loggingOut: true);
    }

    public static void RemoveBeta4Client(IConnection connection)
    {
        if (connection == null)
        {
            return;
        }

        var client = FindBeta4Client(connection);
        if (client == null)
        {
            return;
        }

        Intersect.Core.ApplicationContext.Context.Value?.Logger.LogDebug(
            string.IsNullOrWhiteSpace(client.Name)

                //? $"Client disconnected ({(client.IsEditor ? "[editor]" : "[client]")})"
                // TODO: Transmit client information on network start so we can determine editor vs client
                ? $"Client disconnected ([menu])"
                : $"Client disconnected ({client.Name}->{client.Entity?.Name ?? "[editor]"})"
        );

        client.Disconnect();
    }

    public static Client FindBeta4Client(IConnection connection)
    {
        lock (Globals.ClientLock)
        {
            return Globals.Clients.Find(client => client?.Connection == connection);
        }
    }

    public void FailedAttempt()
    {
        AccountAttempts++;
        ResetTimeout();
    }

    public void ResetTimeout()
    {
        TimeoutMs = Timing.Global.Milliseconds + 5000;
        if (AccountAttempts > 3)
        {
            TimeoutMs += 1000 * AccountAttempts;
        }
    }

    public void SendPackets()
    {
        while (mSendPacketQueue.TryDequeue(out Tuple<IPacket, TransmissionMode, long> tuple))
        {
            if (Connection != null)
            {
                var packet = tuple.Item1;
                var mode = tuple.Item2;

                try
                {
                    if (packet is AbstractTimedPacket timedPacket)
                    {
                        timedPacket.UpdateTiming();
                    }
                    Connection?.Send(packet, mode);
                    if (Options.Instance.Metrics.Enable)
                    {
                        if (!PacketSender.SentPacketTypes.ContainsKey(packet.GetType().Name))
                        {
                            PacketSender.SentPacketTypes.TryAdd(packet.GetType().Name, 0);
                        }
                        PacketSender.SentPacketTypes[packet.GetType().Name]++;
                        PacketSender.SentPackets++;
                        PacketSender.SentBytes += packet.Data.Length;
                        MetricsRoot.Instance.Network.TotalSentPacketProcessingTime.Record(Timing.Global.Milliseconds - tuple.Item3);
                    }
                }
                catch (Exception exception)
                {
                    var packetType = packet.GetType().Name;
                    var packetMessage =
                        $"Sending Packet Error! [Packet: {packetType} | User: {this.Name ?? ""} | Player: {this.Entity?.Name ?? ""} | IP {this.Ip}]";

                    // TODO: Re-combine these once we figure out how to prevent the OutOfMemoryException that happens occasionally
                    ApplicationContext.Logger.LogError(packetMessage);
                    ApplicationContext.Logger.LogError(new ExceptionInfo(exception));
                    if (exception.InnerException != null)
                    {
                        ApplicationContext.Logger.LogError(new ExceptionInfo(exception.InnerException));
                    }

                    // Make the call that triggered the OOME in the first place so that we know when it stops happening
                    ApplicationContext.Logger.LogError(exception, packetMessage);

#if DIAGNOSTIC
                        this.Disconnect($"Error processing packet type '{packetType}'.");
#else
                    this.Disconnect($"Error sending packet.");
#endif
                    break;
                }
            }
        }
        lock (mSendPacketQueue)
        {
            PacketSendingQueued = false;
        }
    }

    public void HandlePackets()
    {
        if (_crashing)
        {
            return;
        }

        var banned = false;
        if (Connection != null)
        {
            if (!mBanChecked)
            {
                if (string.IsNullOrEmpty(Connection?.Ip))
                {
                    banned = true;
                }
                if (!banned && !string.IsNullOrEmpty(Database.PlayerData.Ban.CheckBan(Connection.Ip.Trim())) && Options.Instance.SecurityOpts.CheckIp(Connection.Ip.Trim()))
                {
                    banned = true;
                }
                if (banned)
                {
                    Disconnect("Banned");
                }

                mBanChecked = true;
            }

            if (banned)
            {
                return;
            }

            while (HandlePacketQueue.TryDequeue(out IPacket packet))
            {
                if (_crashing)
                {
                    return;
                }

                if (Connection == null)
                {
                    continue;
                }

                try
                {
                    PacketHandler.Instance.ProcessPacket(packet, this);
                    if (Options.Instance.Metrics.Enable)
                    {
                        MetricsRoot.Instance.Network.TotalReceivedPacketHandlingTime.Record(
                            Timing.Global.Milliseconds - packet.ReceiveTime
                        );
                    }
                }
                catch (Exception exception)
                {
                    var packetType = packet.GetType().Name;
                    var packetMessage =
                        $"Client Packet Error! [Packet: {packetType} | User: {this.Name ?? ""} | Player: {this.Entity?.Name ?? ""} | IP {this.Ip}]";

                    // TODO: Re-combine these once we figure out how to prevent the OutOfMemoryException that happens occasionally
                    ApplicationContext.Logger.LogError(packetMessage);
                    ApplicationContext.Logger.LogError(new ExceptionInfo(exception));
                    if (exception.InnerException != null)
                    {
                        ApplicationContext.Logger.LogError(new ExceptionInfo(exception.InnerException));
                    }

                    // Make the call that triggered the OOME in the first place so that we know when it stops happening
                    ApplicationContext.Logger.LogError(exception, packetMessage);

#if DIAGNOSTIC
                        this.Disconnect($"Error processing packet type '{packetType}'.");
#else
                    this.Disconnect($"Error processing packet.");
#endif
                    break;
                }
            }
        }
        lock (HandlePacketQueue)
        {
            PacketHandlingQueued = false;
        }
    }

    public async Task LogAndDisconnect(Guid? playerId, string? meta = default)
    {
        _crashing = true;
        HandlePacketQueue.Clear();

        var history = await UserActivityHistory.LogActivityAsync(
            User?.Id ?? default,
            playerId ?? Entity?.Id ?? default,
            Ip,
            IsEditor ? UserActivityHistory.PeerType.Editor : UserActivityHistory.PeerType.Client,
            UserActivityHistory.UserAction.DisconnectDatabaseFailure,
            meta
        );

        var message = history?.Id.ToString();
        if (message == default)
        {
            ApplicationContext.Logger.LogError($"Failed to record crash for {User?.Id.ToString() ?? "N/A"}");
        }

        Disconnect(message ?? Strings.Networking.ServerFull, loggingOut: true);
    }

    #region Implementation of IPacketSender

    /// <inheritdoc />
    public bool Send(IPacket packet) => Send(packet, TransmissionMode.All);

    /// <inheritdoc />
    public bool Send(IPacket packet, TransmissionMode mode)
    {
        if (Connection == null)
        {
            return false;
        }

        if (packet == default)
        {
            return false;
        }

        mSendPacketQueue.Enqueue(new Tuple<IPacket, TransmissionMode, long>(packet, mode, Timing.Global.Milliseconds));
        lock (mSendPacketQueue)
        {
            if (PacketSendingQueued)
            {
                return true;
            }

            PacketSendingQueued = true;
            EnqueueNetworkTask.Invoke(SendPackets);
        }

        return true;
    }

    #endregion

    public static Action<Action> EnqueueNetworkTask { get; set; } = action => action();
}
