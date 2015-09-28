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
using System.Collections.Generic;
using System.Threading;

namespace Intersect_Server.Classes
{
    public class Resource : Entity
    {
        // Resource Number
        public int ResourceNum = 0;

        //Respawn
        public long RespawnTime = 0;
        
        public Resource(int index, int resourceNum) : base(index)
        {
            ResourceNum = resourceNum;
            ResourceStruct myBase = Globals.GameResources[ResourceNum];
            MyName = myBase.Name;
            MySprite = myBase.InitialGraphic;
            Vital[(int)Enums.Vitals.Health] = Globals.Rand.Next(myBase.MinHP, myBase.MaxHP + 1);
            MaxVital[(int)Enums.Vitals.Health] = Vital[(int)Enums.Vitals.Health];
            Passable = Convert.ToInt32(myBase.WalkableBefore);
            HideName = 1;
        }

        public override void Die()
        {
            ResourceStruct myBase = Globals.GameResources[ResourceNum];
            RespawnTime = Environment.TickCount + Globals.GameResources[ResourceNum].SpawnDuration * 1000;
            MySprite = myBase.EndGraphic;
            Passable = Convert.ToInt32(myBase.WalkableAfter);
            RespawnTime = Environment.TickCount + myBase.SpawnDuration * 1000;
            PacketSender.SendEntityDataToProximity(MyIndex, 3, Globals.Entities[MyIndex]);
            PacketSender.SendEntityPositionToAll(MyIndex, 3, Globals.Entities[MyIndex]);
            PacketSender.SendEntityVitals(MyIndex, 3, Globals.Entities[MyIndex]);
        }

        public void Respawn()
        {
            ResourceStruct myBase = Globals.GameResources[ResourceNum];
            if (myBase.Name == "" || myBase.MaxHP == 0) { return; }
            MySprite = myBase.InitialGraphic;
            Vital[(int)Enums.Vitals.Health] = Globals.Rand.Next(myBase.MinHP, myBase.MaxHP + 1);
            MaxVital[(int)Enums.Vitals.Health] = Vital[(int)Enums.Vitals.Health];
            Passable = Convert.ToInt32(myBase.WalkableBefore);
            PacketSender.SendEntityDataToProximity(MyIndex, 3, Globals.Entities[MyIndex]);
            PacketSender.SendEntityPositionToAll(MyIndex, 3, Globals.Entities[MyIndex]);
            PacketSender.SendEntityVitals(MyIndex, 3, Globals.Entities[MyIndex]);
        }

        public void SpawnResourceItems(int KillerIndex)
        {
            // Drop items
            for (int i = 0; i < Constants.MaxInvItems; i++)
            {
                if (Inventory[i].ItemNum >= 0)
                {
                    // If the resource isn't walkable, spawn it underneath the killer otherwise loot will be unobtainable.
                    if (Globals.GameResources[ResourceNum].WalkableAfter == true)
                    {
                        Globals.GameMaps[CurrentMap].SpawnItem(CurrentX, CurrentY, Inventory[i], Inventory[i].ItemVal);
                    }
                    else
                    {
                        Globals.GameMaps[CurrentMap].SpawnItem(Globals.Entities[KillerIndex].CurrentX, Globals.Entities[KillerIndex].CurrentY, Inventory[i], Inventory[i].ItemVal);
                    }
                }
            }
        }

        public byte[] Data()
        {
            return base.Data();
        }
    }
}
