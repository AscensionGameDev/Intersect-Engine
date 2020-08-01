using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Server.Database;
using Intersect.Server.General;
using Intersect.Server.Maps;

namespace Intersect.Server.Core
{

    public static class ServerLoop
    {
        public static object Lock = new object();
        public static void RunServerLoop()
        {
            try
            {
                var cpsTimer = Globals.Timing.Milliseconds + 1000;
                long cps = 0;
                long minuteTimer = 0;
                var lastGameSave = Globals.Timing.Milliseconds + 60000;
                var lastDbUpdate = DateTime.Now;
                long dbBackupMinutes = 120;
                while (ServerContext.Instance.IsRunning)
                {
                    var timeMs = Globals.Timing.Milliseconds;

                    lock (Lock)
                    {
                        //TODO: If there are no players online then loop slower and save the poor cpu
                        var maps = MapInstance.Lookup.Values.ToArray();

                        //TODO: Could be optimized by keeping a list of active maps or something
                        foreach (MapInstance map in maps)
                        {
                            map.Update(timeMs);
                        }
                    }
                    
                    if (minuteTimer < timeMs)
                    {
                        if (lastDbUpdate.AddMinutes(dbBackupMinutes) < DateTime.Now)
                        {
                            Task.Run(() => DbInterface.BackupDatabase());
                            lastDbUpdate = DateTime.Now;
                        }

                        DbInterface.SavePlayerDatabaseAsync();
                        minuteTimer = timeMs + 60000;
                    }

                    cps++;
                    if (timeMs >= cpsTimer)
                    {
                        Globals.Cps = cps;
                        cps = 0;
                        cpsTimer = timeMs + 1000;
                    }

                    Time.Update();
                    var currentTime = Globals.Timing.Milliseconds;
                    if (Globals.CpsLock && currentTime < timeMs + 10)
                    {
                        var waitTime = (int)(timeMs + 10 - currentTime);
                        Thread.Sleep(waitTime);
                    }

                    if (timeMs > lastGameSave)
                    {
                        Task.Run(() => DbInterface.SaveGameDatabase());
                        lastGameSave = timeMs + 60000;
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(
                    () => Bootstrapper.OnUnhandledException(
                        Thread.CurrentThread.Name, new UnhandledExceptionEventArgs(ex, true)
                    )
                );
            }
            finally
            {
                ServerContext.Instance.RequestShutdown();
            }
        }

    }

}
