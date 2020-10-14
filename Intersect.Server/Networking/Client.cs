using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Intersect.Logging;
using Intersect.Network;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;

namespace Intersect.Server.Networking
{

    public class Client
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

        private ConcurrentQueue<byte[]> mSendQueue = new ConcurrentQueue<byte[]>();

        protected long mTimeout = 20000; //20 seconds

        //Sent Maps
        public Dictionary<Guid, Tuple<long, int>> SentMaps = new Dictionary<Guid, Tuple<long, int>>();

        public Client(IConnection connection = null)
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

        public void SetUser(User user)
        {
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

        public void SendPacket(CerasPacket packet)
        {
            if (mConnection != null)
            {
                mConnection.Send(packet);
            }
        }

        public void Pinged()
        {
            if (mConnection != null)
            {
                mConnectionTimeout = Globals.Timing.Milliseconds + mTimeout;
            }
        }

        public void Disconnect(string reason = "")
        {
            if (mConnection != null)
            {
                Logout();
                mConnection.Dispose();

                return;
            }
        }

        public bool IsConnected()
        {
            return mConnection.IsConnected;
        }

        public string GetIp()
        {
            if (!IsConnected())
            {
                return "";
            }

            return mConnection.Ip;
        }

        public static Client CreateBeta4Client(IConnection connection)
        {
            var client = new Client(connection);
            lock (Globals.ClientLock)
            {
                Globals.Clients.Add(client);
                Globals.ClientLookup.Add(connection.Guid, client);
            }

            return client;
        }

        public void Logout()
        {
            if (Entity == null)
            {
                return;
            }

            Entity.LastOnline = DateTime.Now;

            DbInterface.SavePlayerDatabaseAsync();

            Entity.TryLogout();

            Entity.Client = null;
            Entity = null;
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

            lock (Globals.ClientLock)
            {
                Globals.Clients.Remove(client);
                Globals.ClientLookup.Remove(connection.Guid);
            }

            Log.Debug(
                string.IsNullOrWhiteSpace(client.Name)

                    //? $"Client disconnected ({(client.IsEditor ? "[editor]" : "[client]")})"
                    // TODO: Transmit client information on network start so we can determine editor vs client
                    ? $"Client disconnected ([menu])"
                    : $"Client disconnected ({client.Name}->{client.Entity?.Name ?? "[editor]"})"
            );

            client.Logout();
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

    }

}
