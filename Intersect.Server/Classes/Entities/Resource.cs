using System;
using System.Collections.Generic;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Networking;

namespace Intersect.Server.Entities
{
    public class Resource : EntityInstance
    {
        public bool IsDead;

        // Resource Number
        public ResourceBase Base;

        //Respawn
        public long RespawnTime = 0;

        public Resource(ResourceBase resource) : base()
        {
            Base = resource;
            Name = resource.Name;
            Sprite = resource.Initial.Graphic;
            SetMaxVital(Vitals.Health, Globals.Rand.Next(Math.Min(1, resource.MinHp),
                Math.Max(resource.MaxHp, Math.Min(1, resource.MinHp)) + 1));
            RestoreVital(Vitals.Health);
            Passable = resource.WalkableBefore;
            HideName = true;
        }

        public void Destroy(int dropitems = 0, EntityInstance killer = null)
        {
            Die(dropitems, killer);
            PacketSender.SendEntityDie(this);
            PacketSender.SendEntityLeave(this);
        }

        public override void Die(int dropitems = 100, EntityInstance killer = null)
        {
            base.Die(0, killer);
            Sprite = Base.Exhausted.Graphic;
            Passable = Base.WalkableAfter;
            IsDead = true;
            if (dropitems > 0)
            {
                SpawnResourceItems(killer);
                if (Base.AnimationId != Guid.Empty)
                    PacketSender.SendAnimationToProximity(Base.AnimationId, -1, Guid.Empty, MapId, (byte)X, (byte)Y,
                        (int) Directions.Up);
            }
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
        }

        public void Spawn()
        {
            Sprite = Base.Initial.Graphic;
            if (Base.MaxHp < Base.MinHp) Base.MaxHp = Base.MinHp;
            SetMaxVital(Vitals.Health,Globals.Rand.Next(Base.MinHp, Base.MaxHp + 1));
            RestoreVital(Vitals.Health);
            Passable = Base.WalkableBefore;
            Items.Clear();

            //Give Resource Drops
            var itemSlot = 0;
            foreach (var drop in Base.Drops)
            {
                if (Globals.Rand.Next(1, 10001) <= drop.Chance * 100 && ItemBase.Get(drop.ItemId) != null)
                {
                    var slot = new InventorySlot(itemSlot);
                    slot.Set(new Item(drop.ItemId,drop.Quantity));
                    Items.Add(slot);
                    itemSlot++;
                }
            }

            IsDead = false;
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
            PacketSender.SendEntityVitals(this);
        }

        public void SpawnResourceItems(EntityInstance killer)
        {
            //Find tile to spawn items
            var tiles = new List<TileHelper>();
            for (int x = X - 1; x <= X + 1; x++)
            {
                for (int y = Y - 1; y <= Y + 1; y++)
                {
                    var tileHelper = new TileHelper(MapId, x, y);
                    if (tileHelper.TryFix())
                    {
                        //Tile is valid.. let's see if its open
                        var map = MapInstance.Get(tileHelper.GetMapId());
                        if (map != null)
                        {
                            if (!map.TileBlocked(tileHelper.GetX(), tileHelper.GetY()))
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
                for (int i = 0; i < tiles.Count; i++)
                {
                    if (tiles[i].GetMapId() == killer.MapId && tiles[i].GetX() == killer.X &&
                        tiles[i].GetY() == killer.Y)
                    {
                        selectedTile = tiles[i];
                    }
                }
                if (selectedTile == null)
                {
                    selectedTile = tiles[Globals.Rand.Next(0, tiles.Count)];
                }
                // Drop items
                foreach (var item in Items)
                {
                    if (ItemBase.Get(item.ItemId) != null)
                    {
                        MapInstance.Get(selectedTile.GetMapId())
                            .SpawnItem(selectedTile.GetX(), selectedTile.GetY(), item, item.Quantity);
                    }
                }
            }
            Items.Clear();
        }

        public override void ProcessRegen()
        {
            //For now give npcs/resources 10% health back every regen tick... in the future we should put per-npc and per-resource regen settings into their respective editors.
            if (!IsDead)
            {
                if (Base == null) return;

                var vital = Vitals.Health;

                var vitalId = (int)vital;
                var vitalValue = GetVital(vital);
                var maxVitalValue = GetMaxVital(vital);
                if (vitalValue < maxVitalValue)
                {
                    var vitalRegenRate = Base.VitalRegen / 100f;
                    var regenValue = (int)Math.Max(1, maxVitalValue * vitalRegenRate) * Math.Abs(Math.Sign(vitalRegenRate));
                    AddVital(vital, regenValue);
                }
            }
        }

        public override bool IsPassable()
        {
            return (IsDead & Base.WalkableAfter) || (!IsDead && Base.WalkableBefore);
        }

        public override EntityPacket EntityPacket(EntityPacket packet = null, Client forClient = null)
        {
            if (packet == null) packet = new ResourceEntityPacket();
            packet = base.EntityPacket(packet, forClient);

            var pkt = (ResourceEntityPacket)packet;
            pkt.ResourceId = Base.Id;
            pkt.IsDead = IsDead;
            return pkt;
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Resource;
        }
    }
}