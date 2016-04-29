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
using Intersect_Library;

namespace Intersect_Server.Classes.Networking
{
	public static class SocketServer
	{
	    static TcpListener _tcpServer;

	    public static void  Init ()
		{
		    _tcpServer = new TcpListener(IPAddress.Any, Options.ServerPort);
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
                new NetSocket(_tcpServer.EndAcceptTcpClient(ar));
                _tcpServer.BeginAcceptTcpClient(OnClientConnect, null);
                //Globals.GeneralLogs.Add("Client Connection From : " + aContext.ClientAddress.ToString());
            }
            catch (Exception)
            {
                //client DC or server shutting down... nothing to worry about
            }
		}
	}
}

