/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Net;
using System.Net.Sockets;

namespace Intersect_Server.Classes
{
    public class TcpDataClient : TcpClient
    {
        public GameSocket Data { get; set; }
        public TcpClient TcpClient { get; set; }
        public NetworkStream MyStream { get; set; }
        public byte[] readBuff { get; set; }
        public TcpDataClient(TcpClient client)
        {
            TcpClient = client;
            MyStream = TcpClient.GetStream();
            readBuff = new byte[TcpClient.ReceiveBufferSize];
        }
    }
	public static class SocketServer
	{
	     static TcpListener _tcpServer;

	    public static void  Init ()
		{
		    _tcpServer = new TcpListener(IPAddress.Any,Globals.ServerPort);
            _tcpServer.Start();
            _tcpServer.BeginAcceptTcpClient(OnClientConnect, null);
		}

        public static void Stop()
        {
            _tcpServer.Stop();
        }

		private static void OnClientConnect (IAsyncResult ar)
		{
            try
            {
                var client = new TcpDataClient(_tcpServer.EndAcceptTcpClient(ar));
                client.TcpClient.NoDelay = false;
                _tcpServer.BeginAcceptTcpClient(OnClientConnect, null);
                client.Data = new GameSocket();
                client.Data.Sender = client;
                client.Data.OnSendData += Network_OnSendData;
                client.readBuff = new byte[client.TcpClient.ReceiveBufferSize];
                if (client.MyStream != null) { client.MyStream.BeginRead(client.readBuff, 0, client.TcpClient.ReceiveBufferSize, OnReceiveData, client); }
                //Globals.GeneralLogs.Add("Client Connection From : " + aContext.ClientAddress.ToString());
            }
            catch (Exception)
            {
                //client DC or server shutting down... nothing to worry about
            }
		}

        static void Network_OnSendData(SendDataEventArgs e)
        {
            try
            {
                ((TcpDataClient)e.socket.Sender).MyStream.Write(e.data, 0, e.data.Length);
            }
            catch (Exception ex)
            {
                Globals.GeneralLogs.Add("Send exception, disconnecting.");
                Globals.GeneralLogs.Add(ex.InnerException.ToString());
                Globals.GeneralLogs.Add(ex.ToString());
                e.socket.HandleDisconnect();
                return;
            }
        }

        private static void OnReceiveData(IAsyncResult ar)
        {
            var newBytes = new byte[1];
            TcpDataClient client = (TcpDataClient)ar.AsyncState;
                try
                {
                    var readbytes = client.MyStream.EndRead(ar);
                    if (readbytes <= 0) { Globals.GeneralLogs.Add("No bytes read, disconnecting."); client.Data.HandleDisconnect(); return; }
                    newBytes = new byte[readbytes];
                    Buffer.BlockCopy(client.readBuff, 0, newBytes, 0, readbytes);
                    client.Data.ReceiveData(newBytes);
                    client.readBuff = new byte[client.TcpClient.ReceiveBufferSize];
                    client.MyStream.BeginRead(client.readBuff, 0, client.TcpClient.ReceiveBufferSize, OnReceiveData, client);
                }
                catch (Exception)
                {
                    Globals.GeneralLogs.Add("Socket end read error.");
                    client.Data.HandleDisconnect();
                    return;
                }
        }



	}
}

