﻿using System;
using System.Collections.Generic;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

namespace Intersect.Server.Entities
{

    public partial class Resource : Entity
    {

        // Resource Number
        public ResourceBase Base;

        //Respawn
        public long RespawnTime = 0;

        public Resource(ResourceBase resource) : base()
        {
            Base = resource;
            Name = resource.Name;
            Sprite = resource.Initial.Graphic;
            SetMaxVital(
                Vital.Health,
                Randomization.Next(
                    Math.Min(1, resource.MinHp), Math.Max(resource.MaxHp, Math.Min(1, resource.MinHp)) + 1
                )
            );

            RestoreVital(Vital.Health);
            Passable = resource.WalkableBefore;
            HideName = true;
        }

        public void Destroy(bool dropItems = false, Entity killer = null)
        {
            lock (EntityLock)
            {
                Die(dropItems, killer);
            }
            
            PacketSender.SendEntityDie(this);
            PacketSender.SendEntityLeave(this);
        }

        public override void Die(bool dropItems = true, Entity killer = null)
        {
            lock (EntityLock)
            {
                base.Die(false, killer);
            }
            
            Sprite = Base.Exhausted.Graphic;
            Passable = Base.WalkableAfter;
            Dead = true;

            if (dropItems)
            {
                SpawnResourceItems(killer);
                if (Base.AnimationId != Guid.Empty)
                {
                    PacketSender.SendAnimationToProximity(
                        Base.AnimationId, -1, Guid.Empty, MapId, X, Y, Direction.Up, MapInstanceId
                    );
                }
            }
 
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
        }

        public void Spawn()
        {
            Sprite = Base.Initial.Graphic;
            if (Base.MaxHp < Base.MinHp)
            {
                Base.MaxHp = Base.MinHp;
            }

            SetMaxVital(Vital.Health, Randomization.Next(Base.MinHp, Base.MaxHp + 1));
            RestoreVital(Vital.Health);
            Passable = Base.WalkableBefore;
            Items.Clear();

            //Give Resource Drops
            var itemSlot = 0;
            foreach (var drop in Base.Drops)
            {
                if (Randomization.Next(1, 10001) <= drop.Chance * 100 && ItemBase.Get(drop.ItemId) != null)
                {
                    var slot = new InventorySlot(itemSlot);
                    slot.Set(new Item(drop.ItemId, drop.Quantity));
                    Items.Add(slot);
                    itemSlot++;
                }
            }

            Dead = false;
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
        }

        public void SpawnResourceItems(Entity killer)
        {
            //Find tile to spawn items
            var tiles = new List<TileHelper>();
            for (var x = X - 1; x <= X + 1; x++)
            {
                for (var y = Y - 1; y <= Y + 1; y++)
                {
                    var tileHelper = new TileHelper(MapId, x, y);
                    if (tileHelper.TryFix())
                    {
                        //Tile is valid.. let's see if its open
                        var mapId = tileHelper.GetMapId();
                        if (MapController.TryGetInstanceFromMap(mapId, MapInstanceId, out var mapInstance))
                        {
                            if (!mapInstance.TileBlocked(tileHelper.GetX(), tileHelper.GetY()))
                            {
                                tiles.Add(tileHelper);
                            }
                            else
                            {
                                if (killer.MapId == tileHelper.GetMapId() &&
                                    killer.X == tileHelper.GetX() &&
                                    killer.Y == tileHelper.GetY())
                                {
                                    tiles.Add(tileHelper);
                                }
                            }
                        }
                    }
                }
            }

            if (tiles.Count > 0)
            {
                TileHelper selectedTile = null;

                //Prefer the players tile, otherwise choose randomly
                for (var i = 0; i < tiles.Count; i++)
                {
                    if (tiles[i].GetMapId() == killer.MapId &&
                        tiles[i].GetX() == killer.X &&
                        tiles[i].GetY() == killer.Y)
                    {
                        selectedTile = tiles[i];
                    }
                }

                if (selectedTile == null)
                {
                    selectedTile = tiles[Randomization.Next(0, tiles.Count)];
                }

                // Drop items
                foreach (var item in Items)
                {
                    if (ItemBase.Get(item.ItemId) != null)
                    {
                        var mapId = selectedTile.GetMapId();
                        if (MapController.TryGetInstanceFromMap(mapId, MapInstanceId, out var mapInstance))
                        {
                            mapInstance.SpawnItem(selectedTile.GetX(), selectedTile.GetY(), item, item.Quantity, killer.Id);
                        }
                    }
                }
            }

            Items.Clear();
        }

        public override void ProcessRegen()
        {
            //For now give npcs/resources 10% health back every regen tick... in the future we should put per-npc and per-resource regen settings into their respective editors.
            if (!IsDead())
            {
                if (Base == null)
                {
                    return;
                }
                
                var vital = Vital.Health;

                var vitalId = (int) vital;
                var vitalValue = GetVital(vital);
                var maxVitalValue = GetMaxVital(vital);
                if (vitalValue < maxVitalValue)
                {
                    var vitalRegenRate = Base.VitalRegen / 100f;
                    var regenValue = (long) Math.Max(1, maxVitalValue * vitalRegenRate) *
                                     Math.Abs(Math.Sign(vitalRegenRate));

                    AddVital(vital, regenValue);
                }
            }
        }

        public override bool IsPassable()
        {
            return IsDead() & Base.WalkableAfter || !IsDead() && Base.WalkableBefore;
        }

        public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
        {
            if (packet == null)
            {
                packet = new ResourceEntityPacket();
            }

            packet = base.EntityPacket(packet, forPlayer);

            var pkt = (ResourceEntityPacket) packet;
            pkt.ResourceId = Base.Id;
            pkt.IsDead = IsDead();

            return pkt;
        }

        public override EntityType GetEntityType()
        {
            return EntityType.Resource;
        }
    }

}
