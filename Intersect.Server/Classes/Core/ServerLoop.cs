using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Server.Core;
using Intersect.Server.General;
using Intersect.Server.Maps;

namespace Intersect.Server
{
    public static class ServerLoop
    {
        public static void RunServerLoop()
        {
            long cpsTimer = Globals.System.GetTimeMs() + 1000;
            long cps = 0;
            long minuteTimer = 0;
            DateTime lastDbUpdate = DateTime.Now;
            long dbBackupMinutes = 120;
            while (ServerContext.IsRunningSafe)
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
                    if (lastDbUpdate.AddMinutes(dbBackupMinutes) < DateTime.Now)
                    {
                        Task.Run(() => LegacyDatabase.BackupDatabase());
                        lastDbUpdate = DateTime.Now;
                    }
                    LegacyDatabase.SavePlayerDatabaseAsync();
                    minuteTimer = timeMs + 60000;
                }
                cps++;
                if (timeMs >= cpsTimer)
                {
                    Globals.Cps = cps;
                    cps = 0;
                    cpsTimer = timeMs + 1000;
                }
                ServerTime.Update();
                var currentTime = Globals.System.GetTimeMs();
                if (Globals.CpsLock && currentTime < timeMs + 10)
                {
                    int waitTime = (int) ((timeMs + 10) - currentTime);
                    Thread.Sleep(waitTime);
                }
            }

            ServerContext.Instance.Dispose();
            //Server is shutting down!!
            //ServerStatic.Shutdown(false);
            //TODO gracefully disconnect all clients
        }
    }
}
