using System;
using System.Collections.Generic;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Networking;

namespace Intersect.Server.Classes.Entities
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
            Passable = Convert.ToInt32(resource.WalkableBefore);
            HideName = 1;
        }

        public void Destroy(int dropitems = 0, EntityInstance killer = null)
        {
            Die(dropitems, killer);
            PacketSender.SendEntityLeave(Id, (int) EntityTypes.Resource, MapId);
        }

        public override void Die(int dropitems = 100, EntityInstance killer = null)
        {
            base.Die(0, killer);
            Sprite = Base.Exhausted.Graphic;
            Passable = Convert.ToInt32(Base.WalkableAfter);
            IsDead = true;
            if (dropitems > 0)
            {
                SpawnResourceItems(killer);
                if (Base.AnimationId != Guid.Empty)
                    PacketSender.SendAnimationToProximity(Base.AnimationId, -1, Guid.Empty, MapId, X, Y,
                        (int) Directions.Up);
            }
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
        }

        public void Spawn()
        {
            Sprite = Base.Initial.Graphic;
            SetMaxVital(Vitals.Health,Globals.Rand.Next(Base.MinHp, Base.MaxHp + 1));
            RestoreVital(Vitals.Health);
            Passable = Convert.ToInt32(Base.WalkableBefore);
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
                    if (ItemBase.Get(item.Id) != null)
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
                foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
                {
                    if ((int) vital < (int) Vitals.VitalCount && !IsFullVital(vital))
                    {
                        AddVital(vital, (int) ((float) GetMaxVital(vital) * .1f));
                    }
                }
            }
        }

        public override byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(base.Data());
            myBuffer.WriteInteger(Convert.ToInt32(IsDead));
            myBuffer.WriteGuid(Base.Id);
            return myBuffer.ToArray();
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Resource;
        }
    }
}