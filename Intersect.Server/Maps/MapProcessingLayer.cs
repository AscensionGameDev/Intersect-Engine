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

        public void Update(long timeMs)
        {
            Console.WriteLine("Updating map processing layer with InstanceLayer: {0}", InstanceLayer);
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

        public void PlayerEnteredMap(Player player)
        {
            //Send Entity Info to Everyone and Everyone to the Entity
            SendMapEntitiesTo(player);
            player.Client?.SentMaps?.Clear();
            PacketSender.SendMapItems(player, mMap.Id);

            AddEntity(player);

            player.LastMapEntered = mMap.Id;
            return; // TODO Alex - Support connecting maps

            /*
            if (SurroundingMaps.Length <= 0)
            {
                return;
            }

            foreach (var t in SurroundingMaps)
            {
                t.SendMapEntitiesTo(player);
                PacketSender.SendMapItems(player, t.Id);
            }
            PacketSender.SendEntityDataToProximity(player, player);
            */
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
    }
}
