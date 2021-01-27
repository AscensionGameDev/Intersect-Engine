using Intersect.Server.Database;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Threading;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Diagnostics;
using Intersect.Server.Entities;
using Amib.Threading;
using System.Collections.Concurrent;

namespace Intersect.Server.Core
{
    internal sealed partial class LogicService
    {

        internal sealed class LogicThread : Threaded<ServerContext>
        {
            public readonly object LogicLock = new object();

            public readonly SmartThreadPool LogicPool = new SmartThreadPool(20000, Options.Instance.Processing.MaxLogicThreads, Options.Instance.Processing.MinLogicThreads);

            public readonly ConcurrentQueue<MapInstance> MapUpdateQueue = new ConcurrentQueue<MapInstance>();

            public readonly ConcurrentQueue<MapInstance> MapProjectileUpdateQueue = new ConcurrentQueue<MapInstance>();

            public readonly HashSet<Guid> ActiveMaps = new HashSet<Guid>();

            public LogicThread() : base("ServerLogic")
            {
            }

            private void AddToQueue(MapInstance map)
            {
                if (Options.Instance.Processing.MapUpdateInterval != Options.Instance.Processing.ProjectileUpdateInterval)
                {
                    MapProjectileUpdateQueue.Enqueue(map);
                }
                MapUpdateQueue.Enqueue(map);
                ActiveMaps.Add(map.Id);
            }

            private void UpdateMap(MapInstance map, bool onlyProjectiles)
            {
                try
                {
                    if (onlyProjectiles)
                    {
                        map.UpdateProjectiles(Globals.Timing.Milliseconds);
                        if (ActiveMaps.Contains(map.Id))
                        {
                            MapProjectileUpdateQueue.Enqueue(map);
                        }
                    }
                    else
                    {
                        map.Update(Globals.Timing.Milliseconds);
                        if (ActiveMaps.Contains(map.Id))
                        {
                            MapUpdateQueue.Enqueue(map);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    //Ignore if this pool is being shut down
                }
                catch (Exception exception)
                {
                    ServerContext.DispatchUnhandledException(exception);
                }
            }

            protected override void ThreadStart(ServerContext serverContext)
            {
                if (serverContext == null)
                {
                    throw new ArgumentNullException(nameof(serverContext));
                }

                try
                {
                    var sw = Stopwatch.StartNew();
                    var swCpsTimer = sw.ElapsedMilliseconds + 1000;
                    long swCps = 0;

                    long updateTimer = 0;

                    var processedMaps = new HashSet<Guid>();
                    var sourceMaps = new HashSet<Guid>();
                    var players = 0;

                    while (ServerContext.Instance.IsRunning)
                    {
                        var startTime = sw.ElapsedMilliseconds;
                        var timeMs = Globals.Timing.Milliseconds;
                        
                        if (timeMs > updateTimer)
                        {
                            //Resync Active Maps By Scanning Players and Their Surrounding Maps
                            players = 0;
                            processedMaps.Clear();
                            sourceMaps.Clear();
                            foreach (var player in Player.OnlineList)
                            {
                                if (player != null)
                                    players++;

                                var plyrMap = player?.MapId ?? Guid.Empty;
                                if (plyrMap != Guid.Empty && !sourceMaps.Contains(plyrMap))
                                {
                                    var mapInstance = MapInstance.Get(plyrMap);
                                    if (mapInstance != null)
                                    {
                                        foreach (var map in mapInstance.GetSurroundingMaps(true))
                                        {
                                            if (!processedMaps.Contains(map.Id))
                                            {
                                                if (!ActiveMaps.Contains(map.Id))
                                                {
                                                    AddToQueue(map);
                                                }
                                                processedMaps.Add(map.Id);
                                            }
                                        }
                                    }
                                    sourceMaps.Add(plyrMap);
                                }
                            }

                            //Uncomment To Stress (Keep all maps active)
                            //foreach (var map in MapInstance.Lookup)
                            //{
                            //    if (!processedMaps.Contains(map.Key))
                            //    {
                            //        processedMaps.Add(map.Key);
                            //        if (!ActiveMaps.Contains(map.Key))
                            //        {
                            //            AddToQueue((MapInstance)map.Value);
                            //        }
                            //    }
                            //}

                            //Remove any Active Maps that we didn't deem neccessarry of processing
                            foreach (var map in ActiveMaps.ToArray())
                            {
                                if (!processedMaps.Contains(map))
                                {
                                    ActiveMaps.Remove(map);
                                }
                            }

                            //End Resync of Active Maps

                            updateTimer = timeMs + 250;
                        }

                        lock (LogicLock)
                        {
                            if (Options.Instance.Processing.MapUpdateInterval != Options.Instance.Processing.ProjectileUpdateInterval)
                            {
                                while (MapProjectileUpdateQueue.TryPeek(out MapInstance result) && result.LastProjectileUpdateTime + Options.Instance.Processing.ProjectileUpdateInterval < timeMs)
                                {
                                    if (MapProjectileUpdateQueue.TryDequeue(out MapInstance sameResult))
                                    {
                                        LogicPool.QueueWorkItem(UpdateMap, sameResult, true);
                                    }
                                }
                            }

                            while (MapUpdateQueue.TryPeek(out MapInstance result) && result.LastUpdateTime + Options.Instance.Processing.MapUpdateInterval < timeMs)
                            {
                                if (MapUpdateQueue.TryDequeue(out MapInstance sameResult))
                                {
                                    LogicPool.QueueWorkItem(UpdateMap, sameResult, false);
                                }
                            }                            
                        }

                        Time.Update();
                        swCps++;

                        var endTime = sw.ElapsedMilliseconds;
                        if (sw.ElapsedMilliseconds > swCpsTimer)
                        {
                            Globals.Cps = swCps;
                            swCps = 0;
                            swCpsTimer = sw.ElapsedMilliseconds + 1000;
                            Console.Title = $"Intersect Server - CPS: {Globals.Cps}, Players: {players}, Active Maps: {ActiveMaps.Count}, Logic Threads: {LogicPool.ActiveThreads} ({LogicPool.InUseThreads} In Use), Pool Queue: {LogicPool.CurrentWorkItemsCount}, Idle: {LogicPool.IsIdle}";
                        }

                        Thread.Yield();
                    }
                    LogicPool.Shutdown();
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
