using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.UI;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;
using Intersect.Server.Entities;
using Intersect.Server.Classes.Maps;

using Newtonsoft.Json;

namespace Intersect.Server.Maps
{
    public class MapProcessingLayer : IDisposable
    {
        [NotMapped] public Guid InstanceLayer;

        private ConcurrentDictionary<Guid, Player> mPlayers = new ConcurrentDictionary<Guid, Player>();

        [NotMapped] private readonly ConcurrentDictionary<Guid, Entity> mEntities = new ConcurrentDictionary<Guid, Entity>();
        private Entity[] mCachedEntities = new Entity[0];

        private MapInstance mMap;

        private long LastUpdateTime;
        
        private MapEntityMovements mEntityMovements = new MapEntityMovements();
        private MapActionMessages mActionMessages = new MapActionMessages();
        private MapAnimations mMapAnimations = new MapAnimations();

        public MapProcessingLayer(MapInstance map, Guid instanceLayer)
        {
            mMap = map;
            InstanceLayer = instanceLayer;
        }

        [NotMapped, JsonIgnore]
        public bool IsDisposed { get; protected set; }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }

        public bool HasPlayersOnMap()
        {
            return !mPlayers.IsEmpty;
        }

        public void Update(long timeMs)
        {
            UpdateEntities(timeMs);

            SendBatchedPacketsToPlayers();

            LastUpdateTime = timeMs;
        }

        public void AddEntity(Entity en)
        {
            if (en != null && !en.IsDead() && en.InstanceLayer == InstanceLayer)
            {
                if (!mEntities.ContainsKey(en.Id))
                {
                    mEntities.TryAdd(en.Id, en);
                    if (en is Player plyr)
                    {
                        mPlayers.TryAdd(plyr.Id, plyr);
                    }
                    mCachedEntities = mEntities.Values.ToArray();
                }
            }
        }

        public void RemoveEntity(Entity en)
        {
            mEntities.TryRemove(en.Id, out var result);
            if (mPlayers.ContainsKey(en.Id))
            {
                mPlayers.TryRemove(en.Id, out var pResult);
            }
            mCachedEntities = mEntities.Values.ToArray();
        }

        public List<Entity> GetEntities(bool includeSurroundingMaps = false)
        {
            var entities = new List<Entity>();

            foreach (var en in mEntities)
                entities.Add(en.Value);

            // ReSharper disable once InvertIf
            if (includeSurroundingMaps)
            {
                foreach (var map in mMap.GetSurroundingMaps(false))
                {
                    // TODO Alex: Support all entities with one call
                    entities.AddRange(map.GetRelevantProcessingLayer(InstanceLayer).GetEntities());
                    entities.AddRange(map.GetEntities());
                }
            }

            return entities;
        }

        public Entity[] GetCachedEntities()
        {
            return mCachedEntities;
        }

        public void PlayerEnteredMap(Player player)
        {
            //Send Entity Info to Everyone and Everyone to the Entity
            SendMapEntitiesTo(player);
            player.Client?.SentMaps?.Clear();
            PacketSender.SendMapItems(player, mMap.Id);

            AddEntity(player);

            player.LastMapEntered = mMap.Id;
            PacketSender.SendEntityDataToProximity(player, player);

            var surroundingMaps = mMap.GetSurroundingMaps();
            if (surroundingMaps.Length <= 0)
            {
                return;
            }

            foreach (var map in surroundingMaps)
            {
                map.GetRelevantProcessingLayer(InstanceLayer).SendMapEntitiesTo(player);
                PacketSender.SendMapItems(player, map.Id);
            }
            PacketSender.SendEntityDataToProximity(player, player);
        }

        public void SendMapEntitiesTo(Player player)
        {
            if (player != null)
            {
                PacketSender.SendMapEntitiesTo(player, mEntities);
                if (player.MapId == mMap.Id && player.InstanceLayer == InstanceLayer)
                {
                    player.SendEvents();
                }
            }
        }

        public ICollection<Player> GetPlayersOnMap()
        {
            return mPlayers.Values;
        }

        public ICollection<Player> GetAllRelevantPlayers()
        {
            var allPlayers = new List<Player>();
            var surroundingMaps = mMap.GetSurroundingMaps();
            foreach (var map in surroundingMaps)
            {
                allPlayers.AddRange(map.GetRelevantProcessingLayer(InstanceLayer).GetPlayersOnMap());
            }
            allPlayers.AddRange(GetPlayersOnMap());
            return allPlayers;
        }

        #region Updates
        private void UpdateEntities(long timeMs)
        {
            // Keep a list of all entities with changed vitals and statusses.
            var vitalUpdates = new List<Entity>();
            var statusUpdates = new List<Entity>();

            foreach (var en in mEntities)
            {
                //Let's see if and how long this map has been inactive, if longer than 30 seconds, regenerate everything on the map
                //TODO, take this 30 second value and throw it into the server config after I switch everything to json
                if (timeMs > LastUpdateTime + 30000)
                {
                    //Regen Everything & Forget Targets
                    if (en.Value is Resource || en.Value is Npc)
                    {
                        en.Value.RestoreVital(Vitals.Health);
                        en.Value.RestoreVital(Vitals.Mana);

                        if (en.Value is Npc npc)
                        {
                            npc.AssignTarget(null);
                        }
                        else
                        {
                            en.Value.Target = null;
                        }
                    }
                }

                en.Value.Update(timeMs);

                // Check to see if we need to send any entity vital and status updates for this entity.
                if (en.Value.VitalsUpdated)
                {
                    vitalUpdates.Add(en.Value);

                    // Send a party update if we're a player with a party.
                    if (en.Value is Player player)
                    {
                        for (var i = 0; i < player.Party.Count; i++)
                        {
                            PacketSender.SendPartyUpdateTo(player.Party[i], player);
                        }
                    }

                    en.Value.VitalsUpdated = false;
                }

                if (en.Value.StatusesUpdated)
                {
                    statusUpdates.Add(en.Value);

                    en.Value.StatusesUpdated = false;
                }
            }

            if (vitalUpdates.Count > 0)
            {
                PacketSender.SendMapEntityVitalUpdate(mMap, vitalUpdates.ToArray());
            }

            if (statusUpdates.Count > 0)
            {
                PacketSender.SendMapEntityStatusUpdate(mMap, statusUpdates.ToArray());
            }
        }

        private void SendBatchedPacketsToPlayers()
        {
            var surrMaps = mMap.GetSurroundingMaps(true);
            var nearbyPlayers = new HashSet<Player>();

            // Get all players in surrounding maps
            foreach (var map in surrMaps)
            {
                foreach (var plyr in map.GetRelevantProcessingLayer(InstanceLayer).GetPlayersOnMap())
                {
                    nearbyPlayers.Add(plyr);
                }
            }
            
            // And all players in the current map
            foreach (var player in GetPlayersOnMap())
            {
                nearbyPlayers.Add(player);
            }

            // And send batched packets out to all nearby players
            mEntityMovements.SendPackets(nearbyPlayers);
            mActionMessages.SendPackets(nearbyPlayers);
            mMapAnimations.SendPackets(nearbyPlayers);
        }
        #endregion
    }
}
