

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