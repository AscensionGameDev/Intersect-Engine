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
        public ResourceBase MyBase;

        //Respawn
        public long RespawnTime = 0;

        public Resource(int index, ResourceBase resource) : base(index, new EntityBase())
        {
            MyBase = resource;
            Name = resource.Name;
            Sprite = resource.InitialGraphic;
            Vital[(int) Vitals.Health] = Globals.Rand.Next(Math.Min(1, resource.MinHp),
                Math.Max(resource.MaxHp, Math.Min(1, resource.MinHp)) + 1);
            MaxVital[(int) Vitals.Health] = Vital[(int) Vitals.Health];
            Passable = Convert.ToInt32(resource.WalkableBefore);
            HideName = 1;
        }

        public void Destroy(int dropitems = 0, EntityInstance killer = null)
        {
            Die(dropitems, killer);
            PacketSender.SendEntityLeave(MyIndex, (int) EntityTypes.Resource, Map);
        }

        public override void Die(int dropitems = 100, EntityInstance killer = null)
        {
            base.Die(0, killer);
            Sprite = MyBase.EndGraphic;
            Passable = Convert.ToInt32(MyBase.WalkableAfter);
            IsDead = true;
            if (dropitems > 0)
            {
                SpawnResourceItems(killer);
                if (MyBase.Animation > -1)
                    PacketSender.SendAnimationToProximity(MyBase.Animation, -1, -1, Map, X, Y,
                        (int) Directions.Up);
            }
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
        }

        public void Spawn()
        {
            Sprite = MyBase.InitialGraphic;
            Vital[(int) Vitals.Health] = Globals.Rand.Next(Math.Min(1, MyBase.MinHp),
                Math.Max(MyBase.MaxHp, Math.Min(1, MyBase.MinHp)) + 1);
            MaxVital[(int) Vitals.Health] = Vital[(int) Vitals.Health];
            Passable = Convert.ToInt32(MyBase.WalkableBefore);
            Items.Clear();

            //Give Resource Drops
            var itemSlot = 0;
            foreach (var drop in MyBase.Drops)
            {
                if (Globals.Rand.Next(1, 10001) <= drop.Chance * 100 && ItemBase.Lookup.Get<ItemBase>(drop.ItemNum) != null)
                {
                    var slot = new InventorySlot(itemSlot);
                    slot.Set(new Item(drop.ItemNum,drop.Amount));
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
                    var tileHelper = new TileHelper(Map, x, y);
                    if (tileHelper.TryFix())
                    {
                        //Tile is valid.. let's see if its open
                        var map = MapInstance.Lookup.Get<MapInstance>(tileHelper.GetMap());
                        if (map != null)
                        {
                            if (!map.TileBlocked(tileHelper.GetX(), tileHelper.GetY()))
                            {
                                tiles.Add(tileHelper);
                            }
                            else
                            {
                                if (killer.Map == tileHelper.GetMap() &&
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
                    if (tiles[i].GetMap() == killer.Map && tiles[i].GetX() == killer.X &&
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
                    if (ItemBase.Lookup.Get<ItemBase>(item.ItemNum) != null)
                    {
                        MapInstance.Lookup.Get<MapInstance>(selectedTile.GetMap())
                            .SpawnItem(selectedTile.GetX(), selectedTile.GetY(), item, item.ItemVal);
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
                    if ((int) vital < (int) Vitals.VitalCount && Vital[(int) vital] != MaxVital[(int) vital])
                    {
                        AddVital(vital, (int) ((float) MaxVital[(int) vital] * .1f));
                    }
                }
            }
        }

        public override byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(base.Data());
            myBuffer.WriteInteger(Convert.ToInt32(IsDead));
            myBuffer.WriteInteger(MyBase.Index);
            return myBuffer.ToArray();
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Resource;
        }
    }
}