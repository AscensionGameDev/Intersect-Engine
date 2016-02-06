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
    public class Projectile : Entity
    {
        public int ProjectileNum = 0;
        public int OwnerID = 0;
        public Type OwnerType = null;
        public long TransmittionTimer = 0;
        public int Target = 0;
        public int DistanceTraveled = 0;

        public Projectile(int index, int ownerID, Type ownerType, int projectileNum, int Map, int X, int Y, int Z, int Direction, int target = 0) : base(index)
        {
            ProjectileNum = projectileNum;
            ProjectileStruct myBase = Globals.GameProjectiles[ProjectileNum];
            MyName = myBase.Name;
            OwnerID = ownerID;
            OwnerType = ownerType;
            MySprite = Globals.Entities[OwnerID].MySprite;
            //MySprite = Globals.GameAnimations[myBase.Animation].UpperAnimSprite;
            Stat[(int)Enums.Stats.Speed] = (5001 - myBase.Speed) / 100;
            TransmittionTimer = Environment.TickCount + (5001 - myBase.Speed);
            Vital[(int)Enums.Vitals.Health] = 1;
            MaxVital[(int)Enums.Vitals.Health] = 1;
            CurrentMap = Map;
            CurrentX = X;
            CurrentY = Y;
            CurrentZ = Z;
            Dir = Direction;
            Target = target;
            Passable = 1;
            HideName = 1;
        }

        public void TryMoveProjectile()
        {
            int c = CanMove(Dir);

            if (Environment.TickCount > TransmittionTimer)
            {
                if (c == 0 && DistanceTraveled < Globals.GameProjectiles[ProjectileNum].Range) //No collision so move.
                {
                    Move(Dir, null);
                    DistanceTraveled++;
                }
                else
                {
                    if (c == 2) //Player
                    {
                        TryAttack(Target, true);
                    }
                    else //Any other target
                    {
                        if (OwnerType == typeof(Player))
                        {
                            TryAttack(Target, true);
                        }
                    }
                    Globals.GameMaps[CurrentMap].RemoveProjectile(this);
                    PacketSender.SendEntityLeave(MyIndex, (int)Enums.EntityTypes.Projectile, CurrentMap);
                    Globals.Entities[MyIndex] = null;
                }
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(base.Data());
            bf.WriteInteger(ProjectileNum);
            bf.WriteInteger(Stat[(int)Enums.Stats.Speed]);
            bf.WriteInteger(Dir);
            bf.WriteInteger(Target);
            return bf.ToArray();
        }
    }
}
