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

	public class Network
	{
	    readonly TcpListener _tcpServer;

	    public Network ()
		{
		    _tcpServer = new TcpListener(IPAddress.Any,Globals.ServerPort);
            _tcpServer.Start();
            _tcpServer.BeginAcceptTcpClient(OnClientConnect, null);
		}

		private void OnClientConnect (IAsyncResult ar)
		{
            var client = _tcpServer.EndAcceptTcpClient(ar);
            client.NoDelay = false;
            _tcpServer.BeginAcceptTcpClient(OnClientConnect, null);
			var tempIndex = FindOpenSocket ();
		    if (tempIndex > -1) {
				Globals.Clients [tempIndex] = new Client (tempIndex, Globals.FindOpenEntity (), client);
				var entityIndex = Globals.Clients [tempIndex].EntityIndex;
                Globals.Entities[entityIndex] = new Player(entityIndex, Globals.Clients[tempIndex]);
                Globals.GeneralLogs.Add("Client connected using client index of " + tempIndex);
			} else {
                Globals.GeneralLogs.Add("Rejecting client due to lack of space.");
			}
		}

		public void RunServer ()
		{
		}

	    static int FindOpenSocket ()
		{
			for (var i = 0; i < Globals.Clients.Count; i++) {
				if (Globals.Clients [i] == null) {
					//return i; --Need to fix before allowing the reuse of indices.
				} else if (i == Globals.Clients.Count - 1) {
					Globals.Clients.Add (null);
					Globals.ClientThread.Add (null);
					return Globals.Clients.Count - 1;
				}
			}
			Globals.ClientThread.Add (null);
			Globals.Clients.Add (null);
			return Globals.Clients.Count - 1;
		}
	}
}

