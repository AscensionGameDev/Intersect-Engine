using WebSocketSharp.Server;

namespace Intersect.Server.Networking.Websockets
{
    public static class WebSocketNetwork
    {
        private static WebSocketSharp.Server.WebSocketServer sListener;

        public static void Init(int port)
        {
            sListener = new WebSocketSharp.Server.WebSocketServer(port);
            sListener.AddWebSocketService<SharpServerService>("/Intersect");
            sListener.Start();
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
            var connection = new WebSocketConnection(Context, this);
        }
    }
}