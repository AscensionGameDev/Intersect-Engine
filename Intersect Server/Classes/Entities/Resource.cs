using System;
using System.Collections.Generic;
using Intersect;
using Intersect.GameObjects;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;

namespace Intersect_Server.Classes.Entities
{
    public class Resource : Entity
    {
        public bool IsDead;
        // Resource Number
        public ResourceBase MyBase;

        //Respawn
        public long RespawnTime = 0;

        public Resource(int index, ResourceBase resource) : base(index)
        {
            MyBase = resource;
            MyName = resource.Name;
            MySprite = resource.InitialGraphic;
            Vital[(int) Vitals.Health] = Globals.Rand.Next(Math.Min(1, resource.MinHP),
                Math.Max(resource.MaxHP, Math.Min(1, resource.MinHP)) + 1);
            MaxVital[(int) Vitals.Health] = Vital[(int) Vitals.Health];
            Passable = Convert.ToInt32(resource.WalkableBefore);
            HideName = 1;
        }

        public void Destroy(bool dropitems = false, Entity killer = null)
        {
            Die(dropitems, killer);
            PacketSender.SendEntityLeave(MyIndex, (int) EntityTypes.Resource, CurrentMap);
        }

        public override void Die(bool dropitems = false, Entity killer = null)
        {
            base.Die(false, killer);
            MySprite = MyBase.EndGraphic;
            Passable = Convert.ToInt32(MyBase.WalkableAfter);
            IsDead = true;
            if (dropitems)
            {
                SpawnResourceItems(killer);
                if (MyBase.Animation > -1)
                    PacketSender.SendAnimationToProximity(MyBase.Animation, -1, -1, CurrentMap, CurrentX, CurrentY,
                        (int) Directions.Up);
            }
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
        }

        public void Spawn()
        {
            MySprite = MyBase.InitialGraphic;
            Vital[(int) Vitals.Health] = Globals.Rand.Next(Math.Min(1, MyBase.MinHP),
                Math.Max(MyBase.MaxHP, Math.Min(1, MyBase.MinHP)) + 1);
            MaxVital[(int) Vitals.Health] = Vital[(int) Vitals.Health];
            Passable = Convert.ToInt32(MyBase.WalkableBefore);
            Inventory.Clear();
            //Give Resource Drops
            for (int n = 0; n < Options.MaxNpcDrops; n++)
            {
                if (Globals.Rand.Next(1, 101) <= MyBase.Drops[n].Chance)
                {
                    Inventory.Add(new MapItemInstance(MyBase.Drops[n].ItemNum, MyBase.Drops[n].Amount, -1));
                }
            }

            IsDead = false;
            PacketSender.SendEntityDataToProximity(this);
            PacketSender.SendEntityPositionToAll(this);
            PacketSender.SendEntityVitals(this);
        }

        public void SpawnResourceItems(Entity killer)
        {
            //Find tile to spawn items
            var tiles = new List<TileHelper>();
            for (int x = CurrentX - 1; x <= CurrentX + 1; x++)
            {
                for (int y = CurrentY - 1; y <= CurrentY + 1; y++)
                {
                    var tileHelper = new TileHelper(CurrentMap, x, y);
                    if (tileHelper.TryFix())
                    {
                        //Tile is valid.. let's see if its open
                        var map = MapInstance.GetMap(tileHelper.GetMap());
                        if (map != null)
                        {
                            if (!map.TileBlocked(tileHelper.GetX(), tileHelper.GetY()))
                            {
                                tiles.Add(tileHelper);
                            }
                            else
                            {
                                if (killer.CurrentMap == tileHelper.GetMap() &&
                                    killer.CurrentX == tileHelper.GetX() &&
                                    killer.CurrentY == tileHelper.GetY())
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
                    if (tiles[i].GetMap() == killer.CurrentMap && tiles[i].GetX() == killer.CurrentX &&
                        tiles[i].GetY() == killer.CurrentY)
                    {
                        selectedTile = tiles[i];
                    }
                }
                if (selectedTile == null)
                {
                    selectedTile = tiles[Globals.Rand.Next(0, tiles.Count)];
                }
                // Drop items
                foreach (var item in Inventory)
                {
                    if (ItemBase.GetItem(item.ItemNum) != null)
                    {
                        MapInstance.GetMap(selectedTile.GetMap())
                            .SpawnItem(selectedTile.GetX(), selectedTile.GetY(), item, item.ItemVal);
                    }
                }
            }
            Inventory.Clear();
        }

        public override byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(base.Data());
            myBuffer.WriteInteger(Convert.ToInt32(IsDead));
            myBuffer.WriteInteger(MyBase.Id);
            return myBuffer.ToArray();
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.Resource;
        }
    }
}