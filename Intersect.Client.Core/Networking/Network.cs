using System.Diagnostics;
using Intersect.Client.Core;
using Intersect.Client.Framework.Network;
using Intersect.Client.General;
using Intersect.Client.Interface.Shared;
using Intersect.Client.Localization;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Network;
using Intersect.Network.Events;
using Microsoft.Extensions.Logging;

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
            ApplicationContext.Context.Value?.Logger.LogTrace(exception, "Error closing socket");
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
                ApplicationContext.Context.Value?.Logger.LogDebug("Disconnect already queued, skipping");
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
                            ApplicationContext.Context.Value?.Logger.LogDebug("Disconnect interrupted, skipping");
                            return;
                        }

                        Globals.SoftLogout = true;
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
        Globals.SoftLogout = false;
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
            ? Strings.Errors.LostConnection.ToString()
            : Strings.Errors.DisconnectionEvent.ToString(connectionEventArgs.EventId);

        // if we did a soft logout we don't want to show the error message
        if (Globals.SoftLogout)
        {
            Globals.SoftLogout = false;
        }
        else
        {
            Interface.Interface.ShowAlert(message, alertType: AlertType.Information);
            Fade.Cancel();
        }

        Globals.WaitingOnServer = false;

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