/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;


namespace Intersect_Server.Classes.Entities
{
    public class Resource : Entity
    {
        // Resource Number
        public ResourceBase MyBase;

        //Respawn
        public long RespawnTime = 0;

        public bool IsDead;
        
        public Resource(int index, ResourceBase resource) : base(index)
        {
            MyBase = resource;
            MyName = resource.Name;
            MySprite = resource.InitialGraphic;
            Vital[(int)Vitals.Health] = Globals.Rand.Next(resource.MinHP, resource.MaxHP + 1);
            MaxVital[(int)Vitals.Health] = Vital[(int)Vitals.Health];
            Passable = Convert.ToInt32(resource.WalkableBefore);
            HideName = 1;
        }

        public override void Die(bool dropitems = false)
        {
            base.Die(dropitems);
            MySprite = MyBase.EndGraphic;
            Passable = Convert.ToInt32(MyBase.WalkableBefore);
            IsDead = true;
            PacketSender.SendEntityDataToProximity(MyIndex, (int)EntityTypes.Resource, Data(), Globals.Entities[MyIndex]);
            PacketSender.SendEntityPositionToAll(MyIndex, (int)EntityTypes.Resource, Globals.Entities[MyIndex]);
        }

        public void Spawn()
        {
            if (MyBase.Name == "" || MyBase.MaxHP == 0) { return; }
            MySprite = MyBase.InitialGraphic;
            Vital[(int)Vitals.Health] = Globals.Rand.Next(MyBase.MinHP, MyBase.MaxHP + 1);
            MaxVital[(int)Vitals.Health] = Vital[(int)Vitals.Health];
            Passable = Convert.ToInt32(MyBase.WalkableBefore);

            //Give Resource Drops
            for (int n = 0; n < Options.MaxNpcDrops; n++)
            {
                if (Globals.Rand.Next(1, 101) <= MyBase.Drops[n].Chance)
                {
                    Inventory.Add(new MapItemInstance(MyBase.Drops[n].ItemNum,MyBase.Drops[n].Amount));
                }
            }
           

            IsDead = false;
            PacketSender.SendEntityDataToProximity(MyIndex, (int)EntityTypes.Resource, Data(), Globals.Entities[MyIndex]);
            PacketSender.SendEntityPositionToAll(MyIndex, (int)EntityTypes.Resource, Globals.Entities[MyIndex]);
            PacketSender.SendEntityVitals(MyIndex, (int)EntityTypes.Resource, Globals.Entities[MyIndex]);
        }

        public void SpawnResourceItems(int KillerIndex)
        {
            // Drop items
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i].ItemNum >= 0)
                {
                    // If the resource isn't walkable, spawn it underneath the killer otherwise loot will be unobtainable.
                    if (MyBase.WalkableAfter == true)
                    {
                        MapInstance.GetMap(CurrentMap).SpawnItem(CurrentX, CurrentY, Inventory[i], Inventory[i].ItemVal);
                    }
                    else
                    {
                        MapInstance.GetMap(CurrentMap).SpawnItem(Globals.Entities[KillerIndex].CurrentX, Globals.Entities[KillerIndex].CurrentY, Inventory[i], Inventory[i].ItemVal);
                    }
                }
            }
        }

        public byte[] Data()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(Convert.ToInt32(IsDead));
            myBuffer.WriteInteger(MyBase.GetId());
            myBuffer.WriteBytes(base.Data());
            return myBuffer.ToArray();
        }
    }
}
