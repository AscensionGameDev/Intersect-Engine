using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Network;

namespace Intersect.Server.Classes.Core
{
    public static class ServerLoop
    {
        public static void RunServerLoop()
        {
            long cpsTimer = Globals.System.GetTimeMs() + 1000;
            long cps = 0;
            while (Globals.ServerStarted)
            {
                var timeMs = Globals.System.GetTimeMs();
                foreach (MapInstance map in MapInstance.Lookup.Values)
                {
                   map.Update(timeMs);
                }
                cps++;
                if (timeMs >= cpsTimer)
                {
                    Globals.CPS = cps;
                    cps = 0;
                    cpsTimer = timeMs + 1000;
                }
                ServerTime.Update();
                var currentTime = Globals.System.GetTimeMs();
                if (Globals.CPSLock && currentTime < timeMs + 10)
                {
                    int waitTime = (int) ((timeMs + 10) - currentTime);
                    Thread.Sleep(waitTime);
                }
            }

            //Server is shutting down!!
            //TODO gracefully disconnect all clients
        }
    }
}