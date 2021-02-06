using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using Intersect.Configuration;
using Intersect.Editor.General;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Crypto;
using Intersect.Crypto.Formats;
using Intersect.Network.Events;
using Intersect.Core;
using System.Collections.Generic;
using Intersect.Threading;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;

namespace Intersect.Editor.Networking
{

    internal static class Network
    {

        public static bool Connecting;

        public static bool ConnectionDenied;

        public static ClientNetwork EditorLidgrenNetwork;

        internal static PacketHandler PacketHandler { get; private set; }

        public static bool Connected => EditorLidgrenNetwork?.IsConnected ?? false;

        public static void InitNetwork()
        {
            if (EditorLidgrenNetwork == null)
            {
                var logger = Log.Default;
                var packetTypeRegistry = new PacketTypeRegistry(logger);
                if (!packetTypeRegistry.TryRegisterBuiltIn())
                {
                    throw new Exception("Failed to register built-in packets.");
                }

                var packetHandlerRegistry = new PacketHandlerRegistry(packetTypeRegistry, logger);
                var networkHelper = new NetworkHelper(packetTypeRegistry, packetHandlerRegistry);
                PackedIntersectPacket.AddKnownTypes(networkHelper.AvailablePacketTypes);
                var virtualEditorContext = new VirtualEditorContext(networkHelper, logger);
                PacketHandler = new PacketHandler(virtualEditorContext, packetHandlerRegistry);

                var config = new NetworkConfiguration(
                    ClientConfiguration.Instance.Host, ClientConfiguration.Instance.Port
                );

                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("Intersect.Editor.network.handshake.bkey.pub"))
                {
                    var rsaKey = EncryptionKey.FromStream<RsaKey>(stream);
                    Debug.Assert(rsaKey != null, "rsaKey != null");
                    EditorLidgrenNetwork = new ClientNetwork(networkHelper, config, rsaKey.Parameters);
                }

                EditorLidgrenNetwork.Handler = PacketHandler.HandlePacket;
                EditorLidgrenNetwork.OnDisconnected += HandleDc;
                EditorLidgrenNetwork.OnConnectionDenied += delegate
                {
                    Connecting = false;
                    ConnectionDenied = true;
                };
            }

            if (!Connected)
            {
                Connecting = true;
                if (!EditorLidgrenNetwork.Connect())
                {
                    Log.Error("An error occurred while attempting to connect.");
                }
            }
        }

        public static void Update()
        {
            if (!Connected && !Connecting && !ConnectionDenied)
            {
                InitNetwork();
            }
        }

        public static void DestroyNetwork()
        {
            try
            {
                EditorLidgrenNetwork.Close();
                EditorLidgrenNetwork = null;
                PacketHandler.Registry.Dispose();
                PacketHandler = null;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void HandleDc(INetworkLayerInterface sender, ConnectionEventArgs connectionEventArgs)
        {
            DestroyNetwork();
            if (Globals.MainForm != null && Globals.MainForm.Visible)
            {
                if (Globals.MainForm.DisconnectDelegate != null)
                {
                    Globals.MainForm.BeginInvoke(Globals.MainForm.DisconnectDelegate);
                    Globals.MainForm.DisconnectDelegate = null;
                }
            }
            else if (Globals.LoginForm.Visible)
            {
                Connecting = false;
                InitNetwork();
            }
            else
            {
                MessageBox.Show(@"Disconnected!");
                Application.Exit();
            }
        }

        public static void SendPacket(IntersectPacket packet)
        {
            if (EditorLidgrenNetwork != null)
            {
                if (!EditorLidgrenNetwork.Send(packet))
                {
                    throw new Exception("Beta 4 network send failed.");
                }
            }
        }

    }

    internal sealed class VirtualEditorContext : IApplicationContext
    {
        internal VirtualEditorContext(NetworkHelper networkHelper, Logger logger)
        {
            NetworkHelper = networkHelper;
            Logger = logger;
        }

        public bool HasErrors => Network.ConnectionDenied;

        public bool IsDisposed { get; private set; }

        public bool IsStarted => IsRunning || Network.Connecting;

        public bool IsRunning => Network.Connected;

        public ICommandLineOptions StartupOptions => default;

        public Logger Logger { get; }

        public List<IApplicationService> Services { get; } = new List<IApplicationService>();

        public INetworkHelper NetworkHelper { get; }

        public void Dispose() => IsDisposed = true;

        public TApplicationService GetService<TApplicationService>() where TApplicationService : IApplicationService => default;

        public void Start(bool lockUntilShutdown = true) { }

        public LockingActionQueue StartWithActionQueue() => default;
    }
}
