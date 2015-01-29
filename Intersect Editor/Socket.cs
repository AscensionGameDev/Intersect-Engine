using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Intersect_Editor
{
    public class Socket
    {
        TcpClient mySocket;
        NetworkStream myStream;
        PacketHandler packetHandler = new PacketHandler();
        bool wasConnected = false;
        List<byte> myBuffer = new List<byte>();
        public long reconnectTime = 0;
        public bool isConnected;
        public bool isConnecting;


        // Update is called once per frame
        public void Update()
        {
            bool shouldInitSocket = false;
            ByteBuffer buff;
            int packetLen;
            int readAmt;
            if (reconnectTime == -1) { reconnectTime = Environment.TickCount + 10000; }
            if (mySocket == null) { shouldInitSocket = true; }
            if (shouldInitSocket == false && mySocket.Connected == false) { shouldInitSocket = true; }
            if (shouldInitSocket && Environment.TickCount > reconnectTime)
            {
                reconnectTime = long.MaxValue ;
                mySocket = new TcpClient();
                mySocket.BeginConnect("ascensionforums.com", 6000, new AsyncCallback(connectCallback), mySocket);
                isConnecting = true;
            }

            if (!isConnected) { return; }


            try
            {
                byte[] tempBuff = new byte[4096];
                if (myStream.DataAvailable)
                {
                    readAmt = myStream.Read(tempBuff, 0, 4096);
                    if (readAmt > 0)
                    {
                        for (int i = 0; i < readAmt; i++)
                        {
                            myBuffer.Add(tempBuff[i]);
                        }
                    }
                    else
                    {
                        //DISCONNECTED!!??
                    }
                }
            }
            catch
            {
                mySocket = null;
                isConnected = false;
                return;
            }

            if (myBuffer.Count >= 4)
            {
                buff = new ByteBuffer();
                buff.WriteBytes(myBuffer.ToArray());
                while (buff.Length() >= 4)
                {
                    packetLen = buff.ReadInteger(false);
                    if (buff.Length() >= packetLen)
                    {
                        //HandlePacket(buff.ReadBytes (packetLen));
                        buff.ReadInteger();
                        packetHandler.HandlePacket(buff.ReadBytes(packetLen));
                    }
                    else
                    {
                        break;
                    }
                }
                myBuffer.Clear();
                if (buff.Length() > 0) { myBuffer.AddRange(buff.ReadBytes(buff.Length())); }
            }
        }


        public void SendPacket(byte[] packet)
        {
            try
            {
                ByteBuffer buff = new ByteBuffer();
                buff.WriteInteger(packet.Length);
                buff.WriteBytes(packet);
                
                myStream.Write(buff.ToArray(), 0, buff.Count());
            }
            catch (Exception)
            {
                Globals.GameSocket.isConnected = false;
                Globals.GameSocket.mySocket.Close();
            }
        }

        /*mySocket.NoDelay = true;
                myStream = mySocket.GetStream();
                isConnected = true;*/
        void connectCallback(IAsyncResult asyncConnect)
        {
            try
            {
                mySocket.EndConnect(asyncConnect);
                // arriving here means the operation completed
                // (asyncConnect.IsCompleted = true) but not
                // necessarily successfully
                if (mySocket.Connected == false)
                {
                    isConnecting = false;
                    isConnected = false;
                    reconnectTime = -1;
                    return;
                }
                else
                {
                    mySocket.NoDelay = true;
                    myStream = mySocket.GetStream();
                    isConnected = true;
                    isConnecting = false;
                }
            }
            catch (Exception)
            {
                isConnecting = false;
                isConnected = false;
                reconnectTime = -1;
                return;
            }
        }
    }
}
