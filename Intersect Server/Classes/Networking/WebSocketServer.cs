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
using Alchemy;
using Alchemy.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;


namespace Intersect_Server.Classes
{
    public static class WebSocketServer
    {
        private static Alchemy.WebSocketServer _listener;
        public static void Init()
        {
            _listener = new Alchemy.WebSocketServer(false,Globals.ServerPort + 1)
            {
                OnReceive = OnReceive,
                OnConnected = OnConnected,
                OnDisconnect = OnDisconnect,
                TimeOut = new TimeSpan(0, 5, 0)
            };
            string[] stuff = new string[1];
            stuff[0] = "binary";
            _listener.SubProtocols = stuff;
            _listener.Start();
        }
        public static void Stop()
        {
            _listener.Stop();
            _listener = null;
        }
        static void OnConnected(UserContext aContext)
        {
            aContext.Data = new GameSocket();
            ((GameSocket)aContext.Data).Sender = aContext;
            ((GameSocket)aContext.Data).OnSendData += WSNetwork_OnSendData;
            Globals.GeneralLogs.Add("Websocket Client Connection From : " + aContext.ClientAddress.ToString());
        }

        static void WSNetwork_OnSendData(SendDataEventArgs e)
        {
            ((UserContext)e.socket.Sender).Send(e.data, e.data.Length, false);
        }

        static void OnReceive(UserContext aContext)
        {
            var data = aContext.DataFrame.AsRaw().ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                ((GameSocket)aContext.Data).ReceiveData(data[i].Array);
            }
        }
        static void OnDisconnect(UserContext aContext)
        {
            ((GameSocket)aContext.Data).HandleDisconnect();
            Globals.GeneralLogs.Add("Websocket Client Disconnect : " + aContext.ClientAddress.ToString());
        }


    }

}