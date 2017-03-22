

using System;
using System.Net;
using System.Net.Sockets;
using Intersect;

namespace Intersect_Server.Classes.Networking
{
	public static class SocketServer
	{
	    static TcpListener _tcpServer;
	    private static bool _started = false;

	    public static void  Init ()
		{
		    _tcpServer = new TcpListener(IPAddress.Any, Options.ServerPort);
            _tcpServer.Start();
	        _started = true;
            _tcpServer.BeginAcceptTcpClient(OnClientConnect, null);
		}

        public static void Stop()
        {
            _started = false;
            _tcpServer.Stop();
        }

        private static void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                if (_started)
                {
                    var gameSocket = new NetSocket(_tcpServer.EndAcceptTcpClient(ar));
                    _tcpServer.BeginAcceptTcpClient(OnClientConnect, null);
                    gameSocket.Start();
                }
            }
            catch (Exception)
            {
                //client DC or server shutting down... nothing to worry about
            }
        }
    }
}

