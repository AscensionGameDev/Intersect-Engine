namespace Intersect_Client.Classes
{
    public static class EntityManager
    {

        public static Entity AddPlayer(int index)
        {
            var i = index;
            while (Globals.Entities.Count <= index)
            {
                Globals.Entities.Add(null);
            }
            if (Globals.Entities[i] != null) { RemoveEntity(i, 0); }
            Globals.Entities[i] = new Player {CurrentMap = 0, MyIndex = i};
            if (index == Globals.MyIndex)
            {
                Globals.Entities[i].IsLocal = true;
                Globals.Me = (Player)Globals.Entities[i];
            }
            return Globals.Entities[i];
        }

        public static Entity AddEvent(int index)
        {
            var i = index;
            while (Globals.Events.Count <= index)
            {
                Globals.Events.Add(null);
            }
            if (Globals.Events[i] != null) { RemoveEntity(i, 1); }
            Globals.Events[i] = new Player {CurrentMap = 0};
            return Globals.Events[i];
        }

        public static void JoinGame()
        {
            Globals.LoggedIn = true;
        }

        public static void RemoveEntity(int index, int isEvent)
        {
            if (isEvent == 1)
            {
                Globals.Events[index] = null;
            }
            else
            {
                if (index == Globals.MyIndex)
                {
                    Globals.Entities[index] = null;
                }

                else
                {
                    Globals.Entities[index] = null;
                }
            }
        }
    }
}
