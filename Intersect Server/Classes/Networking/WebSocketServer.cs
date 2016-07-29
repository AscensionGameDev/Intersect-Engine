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

using Intersect_Library;
using WebSocketSharp.Server;


namespace Intersect_Server.Classes.Networking
{
    public static class WebSocketServer
    {
        private static WebSocketSharp.Server.WebSocketServer _listener;

        public static void Init()
        {
            _listener = new WebSocketSharp.Server.WebSocketServer(Options.ServerPort + 1);
            _listener.AddWebSocketService<SharpServerService>("/Intersect");
            _listener.Start();
        }

        public static void Stop()
        {
            
        }
    }

    public class SharpServerService : WebSocketBehavior
    {
        public SharpServerService() : base()
        {
            IgnoreExtensions = true;
            Protocol = "binary";
        }

        protected override void OnOpen()
        {
            var gameSocket = new WebSocket(Context);
            gameSocket.Start();
        }
    }

}