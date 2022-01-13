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
using Intersect.Utilities;

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
            public readonly ConcurrentQueue<MapInstance> LayerUpdateQueue = new ConcurrentQueue<MapInstance>();

            /// <summary>
            /// Queue of active maps which maps are added to after being updated. Once a map makes it to the front of the queue they are updated again. 
            /// This queue is only used for projectile updates if the projectile update interval does not match the map update interval in the server config.
            /// </summary>
            public readonly ConcurrentQueue<MapInstance> LayerProjectileUpdateQueue = new ConcurrentQueue<MapInstance>();

            /// <summary>
            /// This is the set of maps determined to be 'active' based on player locations in the game. Our logic recalculates this hashset every 250ms.
            /// When maps are updated they are not added back into the map update queues unless they exist in this hash set.
            /// </summary>
            public readonly Dictionary<Guid, MapInstance> ActiveLayers = new Dictionary<Guid, MapInstance>();

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
                    var swCpsTimer = Timing.Global.Milliseconds + 1000;
                    var lastCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                    var saveServerVariablesTimer = Timing.Global.Milliseconds + Options.Instance.Processing.DatabaseSaveServerVariablesInterval;
                    var metricsTimer = 0l;
                    long swCps = 0;

                    long updateTimer = 0;

                    var processedMapLayers = new HashSet<Guid>();
                    var sourceMapLayer = new HashSet<Guid>();
                    var players = 0;

                    while (ServerContext.Instance.IsRunning)
                    {
                        var startTime = Timing.Global.Milliseconds;


                        if (startTime > updateTimer)
                        {
                            //Resync Active Maps By Scanning Players and Their Surrounding Maps
                            players = 0;
                            processedMapLayers.Clear();
                            sourceMapLayer.Clear();

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
                                if (plyrMap != Guid.Empty 
                                    && !sourceMapLayer.Contains(plyrMap))
                                {
                                    // Queue up each surrounding processing layer of the given player
                                    MapController.GetSurroundingMapInstances(plyrMap, player.MapInstanceId, true)
                                        .ForEach(instance =>
                                        {
                                            if (!processedMapLayers.Contains(instance.Id))
                                            {
                                                if (!ActiveLayers.Keys.Contains(instance.Id))
                                                {
                                                    AddToQueue(instance);
                                                }

                                                globalEntities += instance.GetCachedEntities().Length;

                                                processedMapLayers.Add(instance.Id);
                                            }
                                            sourceMapLayer.Add(instance.Id);
                                        });
                                }
                            }

                            //Refresh list of active maps & their processing layers
                            foreach (var layerId in ActiveLayers.Keys.ToArray())
                            {
                                if (!processedMapLayers.Contains(layerId))
                                {
                                    // Remove the map entirely from the update queue
                                    if (ActiveLayers[layerId] != null && ActiveLayers[layerId].ShouldBeCleaned())
                                    {
                                        ActiveLayers[layerId].RemoveLayerFromController();
                                        ActiveLayers.Remove(layerId);
                                    } else if (ActiveLayers[layerId] == null)
                                    {
                                        ActiveLayers.Remove(layerId);
                                    }
                                }
                            }

                            if (Options.Instance.Metrics.Enable)
                            {
                                MetricsRoot.Instance.Game.ActiveEntities.Record(globalEntities);
                                MetricsRoot.Instance.Game.ActiveEvents.Record(events);
                                MetricsRoot.Instance.Game.ProcessingEvents.Record(eventsProcessing);
                                MetricsRoot.Instance.Game.AutorunEvents.Record(autorunEvents);
                                MetricsRoot.Instance.Game.ActiveMaps.Record(ActiveLayers.Count);
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
                                while (LayerProjectileUpdateQueue.TryPeek(out MapInstance result) && result.LastProjectileUpdateTime + Options.Instance.Processing.ProjectileUpdateInterval <= startTime)
                                {
                                    if (LayerProjectileUpdateQueue.TryDequeue(out MapInstance sameResult))
                                    {
                                        LogicPool.QueueWorkItem(UpdateMap, sameResult, true);
                                    }
                                }
                            }

                            while (LayerUpdateQueue.TryPeek(out MapInstance result) && result.LastRequestedUpdateTime + Options.Instance.Processing.MapUpdateInterval <= startTime)
                            {
                                if (LayerUpdateQueue.TryDequeue(out MapInstance sameResult))
                                {
                                    if (Options.Instance.Metrics.Enable)
                                    {
                                        var delay = Timing.Global.Milliseconds - (result.LastRequestedUpdateTime + Options.Instance.Processing.MapUpdateInterval);
                                        MetricsRoot.Instance.Game.MapQueueUpdateOffset.Record(delay);
                                        result.UpdateQueueStart = Timing.Global.Milliseconds;
                                    }
                                    LogicPool.QueueWorkItem(UpdateMap, sameResult, false);
                                }
                            }                            
                        }

                        Time.Update();
                        swCps++;

                        var endTime = Timing.Global.Milliseconds;
                        if (Timing.Global.Milliseconds > swCpsTimer)
                        {
                            Globals.Cps = swCps;
                            swCps = 0;
                            
                            Console.Title = $"Intersect Server - CPS: {Globals.Cps}, Players: {players}, Active Maps: {ActiveLayers.Count}, Logic Threads: {LogicPool.ActiveThreads} ({LogicPool.InUseThreads} In Use), Pool Queue: {LogicPool.CurrentWorkItemsCount}, Idle: {LogicPool.IsIdle}";

                            if (Options.Instance.Metrics.Enable)
                            {
                                //Get Average CPU Usage for the last second
                                var currentCpuTime = Process.GetCurrentProcess().TotalProcessorTime;
                                var cpuUsedMs = (currentCpuTime - lastCpuTime).TotalMilliseconds;
                                var totalMsPassed = Timing.Global.Milliseconds - (swCpsTimer - 1000);
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
                                if (guild.Value.LastUpdateTime + Options.Instance.Guild.GuildUpdateInterval < Timing.Global.Milliseconds)
                                {
                                    LogicPool.QueueWorkItem(guild.Value.UpdateMemberList);
                                }
                            }

                            swCpsTimer = Timing.Global.Milliseconds + 1000;
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

                            if (Timing.Global.Milliseconds > metricsTimer)
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

                                metricsTimer = Timing.Global.Milliseconds + 5000;
                            }
                        }

                        if (saveServerVariablesTimer < endTime)
                        {
                            DbInterface.Pool.QueueWorkItem(DbInterface.SaveUpdatedServerVariables);
                            saveServerVariablesTimer = Timing.Global.Milliseconds + Options.Instance.Processing.DatabaseSaveServerVariablesInterval;
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
            /// Adds a map processing layer to the map update queues for our logic loop to start processing.
            /// </summary>
            /// <param name="mapInstance">The map instance in which to process in our queues.</param>
            private void AddToQueue(MapInstance mapInstance)
            {
                if (Options.Instance.Processing.MapUpdateInterval != Options.Instance.Processing.ProjectileUpdateInterval)
                {
                    LayerProjectileUpdateQueue.Enqueue(mapInstance);
                }
                LayerUpdateQueue.Enqueue(mapInstance);
                ActiveLayers.Add(mapInstance.Id, mapInstance);
                mapInstance.LastRequestedUpdateTime = Timing.Global.Milliseconds - Options.Instance.Processing.MapUpdateInterval;
            }

            /// <summary>
            /// This function actually runs our map update function on the logic thread pool, and then re-queues our map for future updates if the map is still considered active.
            /// </summary>
            /// <param name="layer">The <see cref="MapInstance"/> our thread updates.</param>
            /// <param name="onlyProjectiles">If true only map projectiles are updated and not the entire map.</param>
            private void UpdateMap(MapInstance layer, bool onlyProjectiles)
            {
                try
                {
                    if (onlyProjectiles)
                    {   
                        layer.UpdateProjectiles(Timing.Global.Milliseconds);
                        if (ActiveLayers.Keys.Contains(layer.Id))
                        {
                            LayerProjectileUpdateQueue.Enqueue(layer);
                        }
                    }
                    else
                    {
                        if (Options.Instance.Metrics.Enable)
                        {
                            var timeBeforeUpdate = Timing.Global.Milliseconds;
                            var desiredMapUpdateTime = layer.LastRequestedUpdateTime + Options.Instance.Processing.MapUpdateInterval;
                            MetricsRoot.Instance.Game.MapUpdateQueuedTime.Record(timeBeforeUpdate - layer.UpdateQueueStart);

                            layer.Update(Timing.Global.Milliseconds);

                            var timeAfterUpdate = Timing.Global.Milliseconds;
                            MetricsRoot.Instance.Game.MapUpdateProcessingTime.Record(timeAfterUpdate - timeBeforeUpdate);
                            MetricsRoot.Instance.Game.MapTotalUpdateTime.Record(timeAfterUpdate - desiredMapUpdateTime);

                            if (ActiveLayers.Keys.Contains(layer.Id))
                            {
                                LayerUpdateQueue.Enqueue(layer);
                            }
                        }
                        else
                        {
                            layer.Update(Timing.Global.Milliseconds);
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
