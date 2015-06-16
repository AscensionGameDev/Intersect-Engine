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
				Console.WriteLine ("Client connected using client index of " + tempIndex);
			} else {
				Console.WriteLine ("Rejecting client due to lack of space.");
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

