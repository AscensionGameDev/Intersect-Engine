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
            /// <summary>
            /// We lock on this in order to stop maps from entering the update queue. This is only done when the editor is saving/modifying game maps or the map grids are being rebuilt.
            /// </summary>
            public readonly object LogicLock = new object();

            /// <summary>
            /// This is our thread pool for handling server/game logic. This includes npcs, event processing, map updating, projectiles, spell casting, etc. 
            /// Min/Max Number of Threads & Idle Timeouts are set via server config.
            /// </summary>
            public readonly SmartThreadPool LogicPool = new SmartThreadPool(Options.Instance.Processing.LogicThreadIdleTimeout, Options.Instance.Processing.MaxLogicThreads, Options.Instance.Processing.MinLogicThreads);

            /// <summary>
            /// Queue of active maps which maps are added to after being updated. Once a map makes it to the front of the queue they are updated again.
            /// </summary>
            public readonly ConcurrentQueue<MapInstance> MapUpdateQueue = new ConcurrentQueue<MapInstance>();

            /// <summary>
            /// Queue of active maps which maps are added to after being updated. Once a map makes it to the front of the queue they are updated again. 
            /// This queue is only used for projectile updates if the projectile update interval does not match the map update interval in the server config.
            /// </summary>
            public readonly ConcurrentQueue<MapInstance> MapProjectileUpdateQueue = new ConcurrentQueue<MapInstance>();

            /// <summary>
            /// This is the set of maps determined to be 'active' based on player locations in the game. Our logic recalculates this hashset every 250ms.
            /// When maps are updated they are not added back into the map update queues unless they exist in this hash set.
            /// </summary>
            public readonly HashSet<Guid> ActiveMaps = new HashSet<Guid>();

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
                    var swCpsTimer = Globals.Timing.Milliseconds + 1000;
                    long swCps = 0;

                    long updateTimer = 0;

                    var processedMaps = new HashSet<Guid>();
                    var sourceMaps = new HashSet<Guid>();
                    var players = 0;

                    while (ServerContext.Instance.IsRunning)
                    {
                        var startTime = Globals.Timing.Milliseconds;


                        if (startTime > updateTimer)
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

                            //Remove any Active Maps that we didn't deem neccessarry of processing
                            foreach (var map in ActiveMaps.ToArray())
                            {
                                if (!processedMaps.Contains(map))
                                {
                                    ActiveMaps.Remove(map);
                                }
                            }

                            //End Resync of Active Maps
                            updateTimer = startTime + 250;
                        }

                        //Check our map update queues. If maps are ready to be updated based on our update intervals set in the server config then tell our thread pool to queue the map update as a work item.
                        lock (LogicLock)
                        {
                            if (Options.Instance.Processing.MapUpdateInterval != Options.Instance.Processing.ProjectileUpdateInterval)
                            {
                                while (MapProjectileUpdateQueue.TryPeek(out MapInstance result) && result.LastProjectileUpdateTime + Options.Instance.Processing.ProjectileUpdateInterval < startTime)
                                {
                                    if (MapProjectileUpdateQueue.TryDequeue(out MapInstance sameResult))
                                    {
                                        LogicPool.QueueWorkItem(UpdateMap, sameResult, true);
                                    }
                                }
                            }

                            while (MapUpdateQueue.TryPeek(out MapInstance result) && result.LastUpdateTime + Options.Instance.Processing.MapUpdateInterval < startTime)
                            {
                                if (MapUpdateQueue.TryDequeue(out MapInstance sameResult))
                                {
                                    LogicPool.QueueWorkItem(UpdateMap, sameResult, false);
                                }
                            }                            
                        }

                        Time.Update();
                        swCps++;

                        var endTime = Globals.Timing.Milliseconds;
                        if (Globals.Timing.Milliseconds > swCpsTimer)
                        {
                            Globals.Cps = swCps;
                            swCps = 0;
                            swCpsTimer = Globals.Timing.Milliseconds + 1000 + 1000;
                            Console.Title = $"Intersect Server - CPS: {Globals.Cps}, Players: {players}, Active Maps: {ActiveMaps.Count}, Logic Threads: {LogicPool.ActiveThreads} ({LogicPool.InUseThreads} In Use), Pool Queue: {LogicPool.CurrentWorkItemsCount}, Idle: {LogicPool.IsIdle}";
                        }

                        Thread.Sleep(1);
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

            /// <summary>
            /// Adds a map to the map update queues for our logic loop to start processing.
            /// </summary>
            /// <param name="map">The map in which we want to add to our update queues.</param>
            private void AddToQueue(MapInstance map)
            {
                if (Options.Instance.Processing.MapUpdateInterval != Options.Instance.Processing.ProjectileUpdateInterval)
                {
                    MapProjectileUpdateQueue.Enqueue(map);
                }
                MapUpdateQueue.Enqueue(map);
                ActiveMaps.Add(map.Id);
            }

            /// <summary>
            /// This function actually runs our map update function on the logic thread pool, and then re-queues our map for future updates if the map is still considered active.
            /// </summary>
            /// <param name="map">The map our thread updates.</param>
            /// <param name="onlyProjectiles">If true only map projectiles are updated and not the entire map.</param>
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

        }
    }
}
