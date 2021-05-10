using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Intersect.ErrorHandling;

using Intersect.Core;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Networking.Lidgren;
using Intersect.Server.Metrics;

namespace Intersect.Server.Networking
{

    public class Client : IPacketSender
    {

        public Guid EditorMap = Guid.Empty;

        //Client Properties
        public bool IsEditor;

        //Network Variables
        private IConnection mConnection;

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

        public Client(IApplicationContext applicationContext, IConnection connection = null)
        {
            this.mConnection = connection;
            mConnectTime = Globals.Timing.Milliseconds;
            mConnectionTimeout = Globals.Timing.Milliseconds + mTimeout;

            PacketSender.SendServerConfig(this);
            PacketSender.SendPing(this);
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

        public long Ping => mConnection.Statistics.Ping;

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

        public User User { get; private set; }

        public List<Player> Characters => User?.Players;

        public Player Entity { get; set; }

        public IApplicationContext ApplicationContext { get; }

        public void SetUser(User user)
        {
            if (user == null)
            {
                User?.TryLogout();
            }

            if (user != null && user != User)
            {
                User.Login(user, mConnection.Ip);
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
            if (mConnection != null)
            {
                mConnectionTimeout = Globals.Timing.Milliseconds + mTimeout;
            }
        }

        public void Disconnect(string reason = "", bool shutdown = false)
        {
            lock (Globals.ClientLock)
            {
                if (mConnection != null)
                {
                    Logout(shutdown);
                        
                    Globals.Clients.Remove(this);
                    Globals.ClientArray = Globals.Clients.ToArray();
                    Globals.ClientLookup.Remove(mConnection.Guid);

                    mConnection.Dispose();
                    mConnection = null;

                    return;
                }
            }
        }

        public bool IsConnected()
        {
            return mConnection?.IsConnected ?? false;
        }

        public string GetIp()
        {
            if (!IsConnected())
            {
                return "";
            }

            return mConnection?.Ip ?? "";
        }

        public static Client CreateBeta4Client(IApplicationContext context, IConnection connection)
        {
            var client = new Client(context, connection);
            lock (Globals.ClientLock)
            {
                Globals.Clients.Add(client);
                Globals.ClientArray = Globals.Clients.ToArray();
                Globals.ClientLookup.Add(connection.Guid, client);
            }

            return client;
        }

        public void Logout(bool force = false)
        {
            var entity = Entity;
            entity?.TryLogout();
            Entity = null;

            if (User != null && User.LoginTime != null)
            {
                User.PlayTimeSeconds += (ulong)(DateTime.UtcNow - (DateTime)User.LoginTime).TotalSeconds;
                User.LoginTime = null;
            }

            if (!force)
            {
                User?.Save();
            }

            SetUser(null);
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

            Log.Debug(
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
                return Globals.Clients.Find(client => client?.mConnection == connection);
            }
        }

        public void FailedAttempt()
        {
            AccountAttempts++;
            ResetTimeout();
        }

        public void ResetTimeout()
        {
            TimeoutMs = Globals.Timing.Milliseconds + 5000;
            if (AccountAttempts > 3)
            {
                TimeoutMs += 1000 * AccountAttempts;
            }
        }

        public void SendPackets()
        {
            while (mSendPacketQueue.TryDequeue(out Tuple<IPacket, TransmissionMode, long> tuple))
            {
                if (mConnection != null)
                {
                    var packet = tuple.Item1;
                    var mode = tuple.Item2;

                    try
                    {
                        if (packet is AbstractTimedPacket timedPacket)
                        {
                            timedPacket.UpdateTiming();
                        }
                        mConnection.Send(packet, mode);
                        if (Options.Instance.Metrics.Enable)
                        {
                            if (!PacketSender.SentPacketTypes.ContainsKey(packet.GetType().Name))
                            {
                                PacketSender.SentPacketTypes.TryAdd(packet.GetType().Name, 0);
                            }
                            PacketSender.SentPacketTypes[packet.GetType().Name]++;
                            PacketSender.SentPackets++;
                            PacketSender.SentBytes += packet.Data.Length;
                            MetricsRoot.Instance.Network.TotalSentPacketProcessingTime.Record(Globals.Timing.Milliseconds - tuple.Item3);
                        }
                    }
                    catch (Exception exception)
                    {
                        var packetType = packet.GetType().Name;
                        var packetMessage =
                            $"Sending Packet Error! [Packet: {packetType} | User: {this.Name ?? ""} | Player: {this.Entity?.Name ?? ""} | IP {this.GetIp()}]";

                        // TODO: Re-combine these once we figure out how to prevent the OutOfMemoryException that happens occasionally
                        Log.Error(packetMessage);
                        Log.Error(new ExceptionInfo(exception));
                        if (exception.InnerException != null)
                        {
                            Log.Error(new ExceptionInfo(exception.InnerException));
                        }

                        // Make the call that triggered the OOME in the first place so that we know when it stops happening
                        Log.Error(exception, packetMessage);

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
            var banned = false;
            if (mConnection != null)
            {
                if (!mBanChecked)
                {
                    if (string.IsNullOrEmpty(mConnection?.Ip))
                    {
                        banned = true;
                    }
                    if (!banned && !string.IsNullOrEmpty(Database.PlayerData.Ban.CheckBan(mConnection.Ip.Trim())) && Options.Instance.SecurityOpts.CheckIp(mConnection.Ip.Trim()))
                    {
                        banned = true;
                    }
                    if (banned)
                    {
                        Disconnect("Banned");
                    }

                    mBanChecked = true;
                }
                if (!banned)
                {
                    while (HandlePacketQueue.TryDequeue(out IPacket packet))
                    {
                        if (mConnection != null)
                        {
                            try
                            {
                                PacketHandler.Instance.ProcessPacket(packet, this);
                                if (Options.Instance.Metrics.Enable)
                                {
                                    MetricsRoot.Instance.Network.TotalReceivedPacketHandlingTime.Record(Globals.Timing.Milliseconds - packet.ReceiveTime);
                                }
                            }
                            catch (Exception exception)
                            {
                                var packetType = packet.GetType().Name;
                                var packetMessage =
                                    $"Client Packet Error! [Packet: {packetType} | User: {this.Name ?? ""} | Player: {this.Entity?.Name ?? ""} | IP {this.GetIp()}]";

                                // TODO: Re-combine these once we figure out how to prevent the OutOfMemoryException that happens occasionally
                                Log.Error(packetMessage);
                                Log.Error(new ExceptionInfo(exception));
                                if (exception.InnerException != null)
                                {
                                    Log.Error(new ExceptionInfo(exception.InnerException));
                                }

                                // Make the call that triggered the OOME in the first place so that we know when it stops happening
                                Log.Error(exception, packetMessage);

#if DIAGNOSTIC
                                this.Disconnect($"Error processing packet type '{packetType}'.");
#else
                                this.Disconnect($"Error processing packet.");
#endif
                                break;
                            }
                        }
                    }
                }
            }
            lock (HandlePacketQueue)
            {
                PacketHandlingQueued = false;
            }
        }

        #region Implementation of IPacketSender

        /// <inheritdoc />
        public bool Send(IPacket packet) => Send(packet, TransmissionMode.All);

        /// <inheritdoc />
        public bool Send(IPacket packet, TransmissionMode mode)
        {
            if (mConnection != null)
            {
                mSendPacketQueue.Enqueue(new Tuple<IPacket, TransmissionMode, long>(packet, mode, Globals.Timing.Milliseconds));
                lock (mSendPacketQueue)
                {
                    if (!PacketSendingQueued)
                    {
                        PacketSendingQueued = true;
                        ServerNetwork.Pool.QueueWorkItem(SendPackets);
                    }
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}
