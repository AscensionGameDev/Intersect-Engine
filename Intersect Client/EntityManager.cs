using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Client
{
    public static class EntityManager
    {

        public static void AddPlayer(int index, string username, string sprite, bool isLocal)
        {
            int i = index;
            while (Globals.entities.Count <= index)
            {
                Globals.entities.Add(null);
            }
            if (Globals.entities[i] != null) { RemoveEntity(i, 0); }
            Globals.entities[i] = new Player();
            Globals.entities[i].myName = username;
            Globals.entities[i].currentMap = 0;
            Globals.entities[i].myIndex = i;
            Globals.entities[i].mySprite = sprite;
            if (isLocal)
            {
                Globals.entities[i].isLocal = true;
            }
        }

        public static void AddEvent(int index, string username, string sprite, bool isLocal)
        {
            int i = index;
            while (Globals.events.Count <= index)
            {
                Globals.events.Add(null);
            }
            if (Globals.events[i] != null) { RemoveEntity(i, 1); }
            Globals.events[i] = new Player();
            Globals.events[i].myName = username;
            Globals.events[i].currentMap = 0;
            Globals.events[i].mySprite = sprite;
        }

        public static void JoinGame()
        {
            Globals.LoggedIn = true;
        }

        public static void RemoveEntity(int index, int isEvent)
        {
            if (isEvent == 1)
            {
                Globals.events[index] = null;
            }
            else
            {
                if (index == Globals.myIndex)
                {
                    Globals.entities[index] = null;
                }

                else
                {
                    Globals.entities[index] = null;
                }
            }
        }
    }
}
