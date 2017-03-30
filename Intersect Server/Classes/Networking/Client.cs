using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intersect;
using Intersect.Logging;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;

namespace Intersect.Server.Classes.Networking
{
    public class Client
    {
        public int EditorMap = -1;
        public Player Entity;
        public int EntityIndex;

        //Client Properties
        public bool IsEditor;

        //Adminastrative punnishments
        public bool Muted = false;
        public string MuteReason = "";

        //Game Incorperation Variables
        public string MyAccount = "";
        public string MyEmail = "";
        public long MyId = -1;
        public string MyPassword = "";
        public string MySalt = "";

        //Network Variables
        private GameSocket mySocket;
        public int Power = 0;
        private ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();

        //Sent Maps
        public Dictionary<int, Tuple<long, int>> SentMaps = new Dictionary<int, Tuple<long, int>>();

        //Processing Thead
        private Thread updateThread;

        public Client(int entIndex, GameSocket socket)
        {
            mySocket = socket;
            EntityIndex = entIndex;
            if (EntityIndex > -1)
            {
                Entity = (Player) Globals.Entities[EntityIndex];
            }
            if (mySocket != null && mySocket.IsConnected())
            {
                PacketSender.SendPing(this);
            }
            updateThread = new Thread(Update);
            updateThread.Start();
        }

        public void SendPacket(byte[] packet)
        {
            var buff = new ByteBuffer();
            if (packet.Length > 800)
            {
                packet = Compression.CompressPacket(packet);
                buff.WriteInteger(packet.Length + 1);
                buff.WriteByte(1); //Compressed
                buff.WriteBytes(packet);
            }
            else
            {
                buff.WriteInteger(packet.Length + 1);
                buff.WriteByte(0); //Not Compressed
                buff.WriteBytes(packet);
            }
            sendQueue.Enqueue(buff.ToArray());
        }

        public void Pinged()
        {
            if (mySocket != null && IsConnected())
            {
                mySocket.Pinged();
            }
        }

        public void Disconnect(string reason = "")
        {
            if (reason == "")
            {
                mySocket.Disconnect();
            }
            else
            {
                //send abort packet and then disconnect?
            }
        }

        public async void Update()
        {
            try
            {
                while (mySocket != null && IsConnected() && Globals.ServerStarted)
                {
                    mySocket.Update();
                    while (sendQueue.TryDequeue(out byte[] data))
                    {
                        if (data != null)
                        {
                            mySocket.SendData(data);
                        }
                    }
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                Log.Trace(ex);
                mySocket.Disconnect();
            }
        }

        public bool IsConnected()
        {
            if (mySocket != null)
            {
                return mySocket.IsConnected();
            }
            else
            {
                return false;
            }
        }

        public string GetIP()
        {
            if (IsConnected())
            {
                return mySocket.GetIP();
            }
            else
            {
                return "";
            }
        }
    }
}