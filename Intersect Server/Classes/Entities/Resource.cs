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
        private ResourceStruct _baseResource;

        //Respawn
        public long RespawnTime = 0;

        public bool IsDead;
        
        public Resource(int index, int resourceNum) : base(index)
        {
            ResourceNum = resourceNum;
            _baseResource = Globals.GameResources[ResourceNum];
            MyName = _baseResource.Name;
            MySprite = _baseResource.InitialGraphic;
            Vital[(int)Enums.Vitals.Health] = Globals.Rand.Next(_baseResource.MinHP, _baseResource.MaxHP + 1);
            MaxVital[(int)Enums.Vitals.Health] = Vital[(int)Enums.Vitals.Health];
            Passable = Convert.ToInt32(_baseResource.WalkableBefore);
            HideName = 1;
        }

        public override void Die(bool dropitems = false)
        {
            base.Die(dropitems);
            MySprite = _baseResource.EndGraphic;
            Passable = Convert.ToInt32(_baseResource.WalkableBefore);
            IsDead = true;
            PacketSender.SendEntityDataToProximity(MyIndex, (int)Enums.EntityTypes.Resource, Data(), Globals.Entities[MyIndex]);
            PacketSender.SendEntityPositionToAll(MyIndex, (int)Enums.EntityTypes.Resource, Globals.Entities[MyIndex]);
        }

        public void Spawn()
        {
            if (_baseResource.Name == "" || _baseResource.MaxHP == 0) { return; }
            MySprite = _baseResource.InitialGraphic;
            Vital[(int)Enums.Vitals.Health] = Globals.Rand.Next(_baseResource.MinHP, _baseResource.MaxHP + 1);
            MaxVital[(int)Enums.Vitals.Health] = Vital[(int)Enums.Vitals.Health];
            Passable = Convert.ToInt32(_baseResource.WalkableBefore);

            //Give Resource Drops
            int Z = 0;
            for (int n = 0; n < Constants.MaxNpcDrops; n++)
            {
                if (Globals.Rand.Next(1, 101) <= _baseResource.Drops[n].Chance)
                {
                    Inventory[Z].ItemNum = _baseResource.Drops[n].ItemNum;
                    Inventory[Z].ItemVal = _baseResource.Drops[n].Amount;
                    Z = Z + 1;
                }
            }
           

            IsDead = false;
            PacketSender.SendEntityDataToProximity(MyIndex, (int)Enums.EntityTypes.Resource, Data(), Globals.Entities[MyIndex]);
            PacketSender.SendEntityPositionToAll(MyIndex, (int)Enums.EntityTypes.Resource, Globals.Entities[MyIndex]);
            PacketSender.SendEntityVitals(MyIndex, (int)Enums.EntityTypes.Resource, Globals.Entities[MyIndex]);
        }

        public void SpawnResourceItems(int KillerIndex)
        {
            // Drop items
            for (int i = 0; i < Inventory.Count; i++)
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
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(Convert.ToInt32(IsDead));
            myBuffer.WriteBytes(base.Data());
            return myBuffer.ToArray();
        }
    }
}
