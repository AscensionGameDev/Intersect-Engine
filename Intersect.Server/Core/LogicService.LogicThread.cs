using Intersect.Server.Database;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Threading;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Intersect.Server.Core
{
    internal sealed partial class LogicService
    {

        internal sealed class LogicThread : Threaded<ServerContext>
        {
            [NotNull] public readonly object LogicLock = new object();

            public LogicThread() : base("ServerLogic")
            {
            }

            protected override void ThreadStart(ServerContext serverContext)
            {
                if (serverContext == null)
                {
                    throw new ArgumentNullException(nameof(serverContext));
                }

                try
                {
                    var cpsTimer = Globals.Timing.TimeMs + 1000;
                    long cps = 0;
                    long minuteTimer = 0;
                    var lastGameSave = Globals.Timing.TimeMs + 60000;
                    var lastDbUpdate = DateTime.Now;
                    long dbBackupMinutes = 120;
                    while (ServerContext.Instance.IsRunning)
                    {
                        var timeMs = Globals.Timing.TimeMs;

                        lock (LogicLock)
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
                        var currentTime = Globals.Timing.TimeMs;
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
                catch (Exception exception)
                {
                    ServerContext.DispatchUnhandledException(exception);
                }
                finally
                {
                    ServerContext.Instance.RequestShutdown();
                }
            }

        }
    }
}
