using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;

namespace Intersect.Server.Classes.Core
{
    public static class ServerLoop
    {
        public static void RunServerLoop()
        {
            long cpsTimer = Globals.System.GetTimeMs() + 1000;
            long cps = 0;
            long minuteTimer = 0;
            DateTime lastDbUpdate = DateTime.Now;
            while (Globals.ServerStarted)
            {
                //TODO: If there are no players online then loop slower and save the poor cpu
                var timeMs = Globals.System.GetTimeMs();
                var maps = MapInstance.Lookup.Values.ToArray();
                //TODO: Could be optimized by keeping a list of active maps or something
                foreach (MapInstance map in maps)
                {
                    map.Update(timeMs);
                }
                if (minuteTimer < timeMs)
                {
                    if (lastDbUpdate.AddMinutes(1) < DateTime.Now)
                    {
                        Task.Run(() => Database.BackupDatabase());
                        lastDbUpdate = DateTime.Now;
                    }
                    minuteTimer = timeMs + 60000;
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
