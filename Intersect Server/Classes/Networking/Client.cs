

using Intersect;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Logging;

namespace Intersect_Server.Classes.Networking
{
    public class Client
    {

        //Game Incorperation Variables
        public string MyAccount = "";
        public string MyEmail = "";
        public string MyPassword = "";
        public string MySalt = "";
        public long MyId = -1;
        public int EntityIndex;
        public Player Entity;
        public int EditorMap = -1;

        //Client Properties
        public bool IsEditor;
        public int Power = 0;

        //Adminastrative punnishments
        public bool Muted = false;
        public string MuteReason = "";

        //Network Variables
        private GameSocket mySocket;
        private ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();

        //Processing Thead
        private Thread updateThread;

        //Sent Maps
        public Dictionary<int, Tuple<long, int>> SentMaps = new Dictionary<int, Tuple<long, int>>();

        public Client(int entIndex, GameSocket socket)
        {
            mySocket = socket;
            EntityIndex = entIndex;
            if (EntityIndex > -1) { Entity = (Player)Globals.Entities[EntityIndex]; }
            if (mySocket != null && mySocket.IsConnected()) { PacketSender.SendPing(this); }
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
                    if (Entity != null)
                    {
                        Entity.Update();
                    }
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
