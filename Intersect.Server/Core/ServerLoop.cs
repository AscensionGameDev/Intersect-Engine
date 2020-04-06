using System;
using System.Collections.Generic;
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

		public static void GetActiveMap(List<MapInstance> activeMapList, Guid mapId, bool checkSide)
		{
			MapInstance map = MapInstance.Get(mapId);
			if (map != null && activeMapList.Contains(map) == false)
			{
				activeMapList.Add(map);
				if (checkSide)
				{
					GetActiveMap(activeMapList, map.Left, false);
					GetActiveMap(activeMapList, map.Up, false);
					GetActiveMap(activeMapList, map.Right, false);
					GetActiveMap(activeMapList, map.Down, false);
				}
			}
		}

        public static void RunServerLoop()
        {
            try
            {
                var cpsTimer = Globals.Timing.TimeMs + 1000;
                long cps = 0;
                long minuteTimer = 0;
                var lastGameSave = Globals.Timing.TimeMs + 60000;
                var lastDbUpdate = DateTime.Now;
                long dbBackupMinutes = 120;
				List<MapInstance> activeMapList = new List<MapInstance>();
                while (ServerContext.Instance.IsRunning)
                {
                    lock (Lock)
                    {
                        //TODO: If there are no players online then loop slower and save the poor cpu
                        var timeMs = Globals.Timing.TimeMs;
                        var maps = MapInstance.Lookup.Values.ToArray();

						//TODO: Could be optimized by keeping a list of active maps or something
						activeMapList.Clear();
						for (var i = 0; i < Globals.Clients.Count; i++)
						{
							if (Globals.Clients[i] == null)
							{
								continue;
							}
							if (Globals.Clients[i].Entity == null)
							{
								continue;
							}
							Guid mapid = Globals.Clients[i].Entity.MapId;
							GetActiveMap(activeMapList, mapid, true);
						}
						foreach (MapInstance map in activeMapList/*maps*/)
                        {
                            map.Update(timeMs);
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
