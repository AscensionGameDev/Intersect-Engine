using System.Threading;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;

namespace Intersect_Server.Classes.Core
{
    public static class ServerLoop
    {
        public static void RunServerLoop()
        {
            long cpsTimer = Globals.System.GetTimeMs() + 1000;
            long cps = 0;
            while (Globals.ServerStarted)
            {
                var maps = MapInstance.Lookup;
                foreach (var map in maps)
                {
                    if (map.Value.Active) map.Value.Update();
                }
                cps++;
                if (Globals.System.GetTimeMs() >= cpsTimer)
                {
                    Globals.CPS = cps;
                    cps = 0;
                    cpsTimer = Globals.System.GetTimeMs() + 1000;
                }
                ServerTime.Update();
                if (Globals.CPSLock)
                {
                    Thread.Sleep(10);
                }
            }

            //Server is shutting down!!
            //TODO gracefully disconnect all clients
            SocketServer.Stop();
            WebSocketServer.Stop();
        }
    }
}