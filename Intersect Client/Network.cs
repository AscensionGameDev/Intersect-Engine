using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Intersect_Client
{
    public static class Network
    {
        public static TcpClient mySocket;
        private static NetworkStream myStream;
        public static bool Connected = false;
        public static bool Connecting = false;
        private static byte[] tempBuff;
        private static List<byte> myBuffer = new List<byte>();

        public static void InitNetwork()
        {
            if (mySocket != null)
            {
                mySocket.Close();
            }

            mySocket = new TcpClient();
            mySocket.NoDelay = true;
            tempBuff = new byte[mySocket.ReceiveBufferSize];
            mySocket.BeginConnect(Globals.ServerHost, Globals.ServerPort, connectCB, null);
            Connecting = true;
        }

        private static void connectCB(IAsyncResult result){
            try
            {
                mySocket.EndConnect(result);
                if (mySocket.Connected)
                {
                    Connected = true;
                    Connecting = false;
                    myStream = mySocket.GetStream();
                    myStream.BeginRead(tempBuff, 0, mySocket.ReceiveBufferSize, receiveCB, null);
                }
                else {
                    Connected = false;
                    Connecting = false;
                }
            }
            catch (Exception)
            {
                Connected = false;
                Connecting = false;
            }
        }

        public static void CheckNetwork()
        {
            if (Connected == false && Connecting == false)
            {
                InitNetwork();
            }
            else
            {
                if (!Connected)
                {
                    //PROBLEM!
                }
            }
        }

        private static void receiveCB(IAsyncResult result)
        {
            int readAmt = 0;
            byte[] receivedData;
            try
            {
                readAmt = myStream.EndRead(result);
                if (readAmt <= 0)
                {
                    handleDC();
                }
                receivedData = new byte[readAmt];
                Buffer.BlockCopy(tempBuff, 0, receivedData, 0, readAmt);
                handleData(receivedData);
                myStream.BeginRead(tempBuff, 0, mySocket.ReceiveBufferSize, receiveCB, null);
            }
            catch (Exception)
            {
                handleDC();
            }
        }

        public static void DestroyNetwork()
        {
            try
            {
                myStream.Close();
                mySocket.Close();
            }
            catch (Exception)
            {

            }
        }

        private static void handleDC()
        {
            System.Windows.Forms.MessageBox.Show("Disconnected!");
            GameMain.isRunning = false;
        }

        public static void sendPacket(byte[] data)
        {
            try
            {
                ByteBuffer buff = new ByteBuffer();
                buff.WriteInteger(data.Length);
                buff.WriteBytes(data);
                myStream.Write(buff.ToArray(), 0, buff.Count());
            }
            catch (Exception ex)
            {
                handleDC();
            }
        }

        private static void handleData(byte[] receivedData)
        {
            ByteBuffer buff;
            int packetLen = receivedData.Length;
            myBuffer.AddRange(receivedData);
            if (myBuffer.Count() >= 4)
            {
                buff = new ByteBuffer();
                buff.WriteBytes(myBuffer.ToArray());
                while (buff.Length() >= 4)
                {
                    packetLen = buff.ReadInteger(false);
                    if (buff.Length() >= packetLen)
                    {
                        buff.ReadInteger();
                        PacketHandler.HandlePacket(buff.ReadBytes(packetLen));
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
    }
}
