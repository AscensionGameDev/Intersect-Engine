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
using Intersect.Server.Metrics;
using Newtonsoft.Json;
using Intersect.Server.Networking;
using Intersect.Server.Networking.Lidgren;
using Intersect.Server.Database.PlayerData.Players;

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
            public readonly SmartThreadPool LogicPool = new SmartThreadPool(
                new STPStartInfo()
                {
                    ThreadPoolName = "LogicPool",
                    IdleTimeout = 20000,
                    MinWorkerThreads = Options.Instance.Processing.MinLogicThreads,
                    MaxWorkerThreads = Options.Instance.Processing.MaxLogicThreads
                }
            );

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
                    var lastCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                    var saveServerVariablesTimer = Globals.Timing.Milliseconds + Options.Instance.Processing.DatabaseSaveServerVariablesInterval;
                    var metricsTimer = 0l;
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

                            //Metrics
                            var globalEntities = 0;
                            var events = 0;
                            var eventsProcessing = 0;
                            var autorunEvents = 0;

                            foreach (var player in Player.OnlineList)
                            {
                                if (player != null)
                                {
                                    players++;
                                    if (Options.Instance.Metrics.Enable)
                                    {
                                        events += player.EventLookup.Count;
                                        eventsProcessing += player.EventLookup.Values.Where(e => e.CallStack?.Count > 0).Count();
                                        autorunEvents += player.CommonAutorunEvents + player.MapAutorunEvents;
                                    }
                                }

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
                                                
                                                globalEntities += map.GetCachedEntities().Length;

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

                            if (Options.Instance.Metrics.Enable)
                            {
                                MetricsRoot.Instance.Game.ActiveEntities.Record(globalEntities);
                                MetricsRoot.Instance.Game.ActiveEvents.Record(events);
                                MetricsRoot.Instance.Game.ProcessingEvents.Record(eventsProcessing);
                                MetricsRoot.Instance.Game.AutorunEvents.Record(autorunEvents);
                                MetricsRoot.Instance.Game.ActiveMaps.Record(ActiveMaps.Count);
                                MetricsRoot.Instance.Game.Players.Record(players);
                                MetricsRoot.Instance.Network.Clients.Record(Globals.Clients?.Count ?? 0);
                            }

                            //End Resync of Active Maps
                            updateTimer = startTime + 250;
                        }

                        //Check our map update queues. If maps are ready to be updated based on our update intervals set in the server config then tell our thread pool to queue the map update as a work item.
                        lock (LogicLock)
                        {
                            if (Options.Instance.Processing.MapUpdateInterval != Options.Instance.Processing.ProjectileUpdateInterval)
                            {
                                while (MapProjectileUpdateQueue.TryPeek(out MapInstance result) && result.LastProjectileUpdateTime + Options.Instance.Processing.ProjectileUpdateInterval <= startTime)
                                {
                                    if (MapProjectileUpdateQueue.TryDequeue(out MapInstance sameResult))
                                    {
                                        LogicPool.QueueWorkItem(UpdateMap, sameResult, true);
                                    }
                                }
                            }

                            while (MapUpdateQueue.TryPeek(out MapInstance result) && result.LastUpdateTime + Options.Instance.Processing.MapUpdateInterval <= startTime)
                            {
                                if (MapUpdateQueue.TryDequeue(out MapInstance sameResult))
                                {
                                    if (Options.Instance.Metrics.Enable)
                                    {
                                        var delay = Globals.Timing.Milliseconds - (result.LastUpdateTime + Options.Instance.Processing.MapUpdateInterval);
                                        MetricsRoot.Instance.Game.MapQueueUpdateOffset.Record(delay);
                                        result.UpdateQueueStart = Globals.Timing.Milliseconds;
                                    }
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
                            
                            Console.Title = $"Intersect Server - CPS: {Globals.Cps}, Players: {players}, Active Maps: {ActiveMaps.Count}, Logic Threads: {LogicPool.ActiveThreads} ({LogicPool.InUseThreads} In Use), Pool Queue: {LogicPool.CurrentWorkItemsCount}, Idle: {LogicPool.IsIdle}";

                            if (Options.Instance.Metrics.Enable)
                            {
                                //Get Average CPU Usage for the last second
                                var currentCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                                var cpuUsedMs = (currentCpuTime - lastCpuTime).TotalMilliseconds;
                                var totalMsPassed = Globals.Timing.Milliseconds - (swCpsTimer - 1000);
                                var cpuUsageTotal = (cpuUsedMs / (Environment.ProcessorCount * totalMsPassed)) * 100f;
                                lastCpuTime = currentCpuTime;

                                MetricsRoot.Instance.Application.Cpu.Record((int)cpuUsageTotal);
                                MetricsRoot.Instance.Application.Memory.Record(Process.GetCurrentProcess().PrivateMemorySize64);

                                MetricsRoot.Instance.Game.Cps.Record(Globals.Cps);


                                //Also Update Networking Metrics
                                MetricsRoot.Instance.Network.TotalBandwidth.Record(PacketHandler.ReceivedBytes + PacketSender.SentBytes);
                                MetricsRoot.Instance.Network.SentBytes.Record(PacketSender.SentBytes);
                                MetricsRoot.Instance.Network.SentPackets.Record(PacketSender.SentPackets);
                                MetricsRoot.Instance.Network.ReceivedBytes.Record(PacketHandler.ReceivedBytes);
                                MetricsRoot.Instance.Network.ReceivedPackets.Record(PacketHandler.ReceivedPackets);
                                MetricsRoot.Instance.Network.AcceptedBytes.Record(PacketHandler.AcceptedBytes);
                                MetricsRoot.Instance.Network.AcceptedPackets.Record(PacketHandler.AcceptedPackets);
                                MetricsRoot.Instance.Network.DroppedBytes.Record(PacketHandler.DroppedBytes);
                                MetricsRoot.Instance.Network.DroppedPackets.Record(PacketHandler.DroppedPackets);

                                PacketSender.ResetMetrics();
                                PacketHandler.ResetMetrics();
                            }

                            //Should we send out guild updates?
                            foreach (var guild in Guild.Guilds)
                            {
                                if (guild.Value.LastUpdateTime + Options.Instance.Guild.GuildUpdateInterval < Globals.Timing.Milliseconds)
                                {
                                    LogicPool.QueueWorkItem(guild.Value.UpdateMemberList);
                                }
                            }

                            swCpsTimer = Globals.Timing.Milliseconds + 1000;
                        }

                        if (Options.Instance.Metrics.Enable)
                        {
                            //Record how our Thread Pools are Operating
                            MetricsRoot.Instance.Threading.LogicPoolActiveThreads.Record(LogicPool.ActiveThreads);
                            MetricsRoot.Instance.Threading.LogicPoolInUseThreads.Record(LogicPool.InUseThreads);
                            MetricsRoot.Instance.Threading.LogicPoolWorkItemsCount.Record(LogicPool.CurrentWorkItemsCount);
                            MetricsRoot.Instance.Threading.NetworkPoolActiveThreads.Record(ServerNetwork.Pool.ActiveThreads);
                            MetricsRoot.Instance.Threading.NetworkPoolInUseThreads.Record(ServerNetwork.Pool.InUseThreads);
                            MetricsRoot.Instance.Threading.NetworkPoolWorkItemsCount.Record(ServerNetwork.Pool.CurrentWorkItemsCount);
                            MetricsRoot.Instance.Threading.DatabasePoolActiveThreads.Record(DbInterface.Pool.ActiveThreads);
                            MetricsRoot.Instance.Threading.DatabasePoolInUseThreads.Record(DbInterface.Pool.InUseThreads);
                            MetricsRoot.Instance.Threading.DatabasePoolWorkItemsCount.Record(DbInterface.Pool.CurrentWorkItemsCount);

                            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIOThreads);
                            ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int availableIOThreads);
                            MetricsRoot.Instance.Threading.SystemPoolInUseWorkerThreads.Record(maxWorkerThreads - availableWorkerThreads);
                            MetricsRoot.Instance.Threading.SystemPoolInUseIOThreads.Record(maxIOThreads - availableIOThreads);

                            if (Globals.Timing.Milliseconds > metricsTimer)
                            {
                                MetricsRoot.Instance.Capture();

                                foreach (var key in PacketSender.SentPacketTypes.Keys)
                                {
                                    PacketSender.SentPacketTypes[key] = 0;
                                }

                                foreach (var key in PacketHandler.AcceptedPacketTypes.Keys)
                                {
                                    PacketHandler.AcceptedPacketTypes[key] = 0;
                                }

                                metricsTimer = Globals.Timing.Milliseconds + 5000;
                            }
                        }

                        if (saveServerVariablesTimer < endTime)
                        {
                            DbInterface.Pool.QueueWorkItem(DbInterface.SaveUpdatedServerVariables);
                            saveServerVariablesTimer = Globals.Timing.Milliseconds + Options.Instance.Processing.DatabaseSaveServerVariablesInterval;
                        }

                        if (Options.Instance.Processing.CpsLock)
                        {
                            Thread.Sleep(1);
                        }
            
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
                map.LastUpdateTime = Globals.Timing.Milliseconds - Options.Instance.Processing.MapUpdateInterval;
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
                        if (Options.Instance.Metrics.Enable)
                        {
                            var timeBeforeUpdate = Globals.Timing.Milliseconds;
                            var desiredMapUpdateTime = map.LastUpdateTime + Options.Instance.Processing.MapUpdateInterval;
                            MetricsRoot.Instance.Game.MapUpdateQueuedTime.Record(timeBeforeUpdate - map.UpdateQueueStart);

                            map.Update(Globals.Timing.Milliseconds);

                            var timeAfterUpdate = Globals.Timing.Milliseconds;
                            MetricsRoot.Instance.Game.MapUpdateProcessingTime.Record(timeAfterUpdate - timeBeforeUpdate);
                            MetricsRoot.Instance.Game.MapTotalUpdateTime.Record(timeAfterUpdate - desiredMapUpdateTime);

                            if (ActiveMaps.Contains(map.Id))
                            {
                                MapUpdateQueue.Enqueue(map);
                            }
                        }
                        else
                        {
                            map.Update(Globals.Timing.Milliseconds);
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
