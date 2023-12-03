using System.Diagnostics;
using Intersect.Client.Core;
using Intersect.Client.Framework.Network;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Events;

namespace Intersect.Client.Networking;

internal static partial class Network
{
    private static readonly object CloseTaskLock = new();

    private static Task? _closeTask;
    private static GameSocket? _socket;

    internal static PacketHandler? PacketHandler { get; set; }

    public static bool IsConnected => Socket.IsConnected;
    public static int Ping => Socket.Ping;

    public static GameSocket Socket
    {
        get => _socket ?? throw new InvalidOperationException("Socket not created.");
        set => _socket = value;
    }


    public static void Close(string reason)
    {
        try
        {
            Socket.Disconnect(reason);
            Socket.Dispose();
        }
        catch (Exception exception)
        {
            Log.Trace(exception);
        }
    }

    public static void DebounceClose(string reason)
    {
        if (!IsConnected)
        {
            return;
        }

        lock (CloseTaskLock)
        {
            if (_closeTask != default)
            {
                Log.Debug("Disconnect already queued, skipping");
                return;
            }

            _closeTask = Task.Delay(2500);
            _closeTask.ContinueWith(
                task =>
                {
                    lock (CloseTaskLock)
                    {
                        if (_closeTask != task)
                        {
                            Log.Debug("Disconnect interrupted, skipping");
                            return;
                        }

                        _closeTask = default;
                        Close(reason);
                    }
                }
            );
        }
    }

    public static void InitNetwork(IClientContext context)
    {
        Debug.Assert(Socket != default);

        Socket.Connected += OnConnected;
        Socket.Disconnected += OnDisconnected;
        Socket.DataReceived += OnDataReceived;
        Socket.ConnectionFailed += OnConnectionFailed;
    }

    public static bool InterruptDisconnectsIfConnected()
    {
        lock (CloseTaskLock)
        {
            _closeTask = default;
            return IsConnected;
        }
    }

    private static void OnConnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
    {
        Globals.WaitingOnServer = false;
    }

    private static void OnConnectionFailed(
        INetworkLayerInterface sender,
        ConnectionEventArgs connectionEventArgs,
        bool denied
    )
    {
        Globals.WaitingOnServer = false;
        if (!denied)
        {
            TryConnect();
        }
    }

    private static void OnDataReceived(IPacket packet)
    {
        PacketHandler?.HandlePacket(packet);
    }

    private static void OnDisconnected(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
    {
        var message = connectionEventArgs.EventId == default
            ? Strings.Errors.lostconnection.ToString()
            : Strings.Errors.DisconnectionEvent.ToString(connectionEventArgs.EventId);

        Interface.Interface.ShowError(message);

        Globals.WaitingOnServer = false;
        Fade.Cancel();

        if (Globals.GameState != GameStates.InGame && Globals.GameState != GameStates.Loading)
        {
            return;
        }

        Globals.ConnectionLost = true;
    }

    public static void SendPacket(IntersectPacket packet)
    {
        Socket.SendPacket(packet);
    }

    public static void TryConnect()
    {
        if (IsConnected)
        {
            return;
        }

        Globals.WaitingOnServer = true;
        Socket.Connect(ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port);
    }

    public static void Update()
    {
        Socket.Update();
    }
}