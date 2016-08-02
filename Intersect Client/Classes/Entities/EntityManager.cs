/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Maps;
using Intersect_Library;

namespace Intersect_Client.Classes.Entities
{
    public static class EntityManager
    {

        public static Entity AddPlayer(int index)
        {
            var i = index;
            Globals.EntitiesToDispose.Remove(index);
            if (Globals.Entities.ContainsKey(i)) {return Globals.Entities[index];}
            Globals.Entities.Add(i,new Player {CurrentMap = 0, MyIndex = i});;
            if (index == Globals.MyIndex)
            {
                Globals.Entities[i].IsLocal = true;
                Globals.Me = (Player)Globals.Entities[i];
            }
            Globals.Entities[i].IsPlayer = true;
            return Globals.Entities[i];
        }

        public static Entity AddGlobalEntity(int index)
        {
            var i = index;
            Globals.EntitiesToDispose.Remove(index);
            if (Globals.Entities.ContainsKey(i)) { return Globals.Entities[index]; }
            Globals.Entities.Add(i,new Entity());
            Globals.Entities[i].MyIndex = i;
            return Globals.Entities[i];
        }

        public static Entity AddLocalEvent(int index, int mapNum)
        {
            var i = index;
            var map = MapInstance.GetMap(mapNum);
            if (map != null)
            {
                //Make sure we are not about to dispose of this entity
                map.LocalEntitiesToDispose.Remove(i);
                if (map.LocalEntities.ContainsKey(i))
                {
                    return map.LocalEntities[index];
                }
                else
                {
                    map.LocalEntities.Add(index,new Event {CurrentMap = 0});
                    return map.LocalEntities[index];
                }
            }
            return null;
        }

        public static Entity AddResource(int index)
        {
            var i = index;
            Globals.EntitiesToDispose.Remove(index);
            if (Globals.Entities.ContainsKey(i)) { return Globals.Entities[index]; }
            Globals.Entities.Add(i,new Resource());
            return Globals.Entities[i];
        }

        public static Entity AddProjectile(int index)
        {
            var i = index;
            Globals.EntitiesToDispose.Remove(index);
            if (Globals.Entities.ContainsKey(i) && Globals.Entities[i].GetType() == typeof(Projectile)) { return Globals.Entities[index]; }
            Globals.Entities.Add(i, new Projectile(i));
            return Globals.Entities[i];
        }

        public static void RemoveEntity(int index, int type, int mapNum)
        {
            if (type != (int)EntityTypes.Event)
            {
                if (Globals.Entities.ContainsKey(index))
                {
                    Globals.Entities[index].Dispose();
                    Globals.EntitiesToDispose.Add(index);
                }
            }
            else
            {
                var map = MapInstance.GetMap(mapNum);
                if (map != null)
                {
                    if (map.LocalEntities.ContainsKey(index))
                    {
                        map.LocalEntities[index].Dispose();
                        map.LocalEntities[index] = null;
                        map.LocalEntities.Remove(index);
                    }
                }
            }
        }
    }
}
