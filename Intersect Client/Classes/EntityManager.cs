namespace Intersect_Client.Classes
{
    public static class EntityManager
    {

        public static void AddPlayer(int index, string username, string sprite, bool isLocal)
        {
            var i = index;
            while (Globals.Entities.Count <= index)
            {
                Globals.Entities.Add(null);
            }
            if (Globals.Entities[i] != null) { RemoveEntity(i, 0); }
            Globals.Entities[i] = new Player {MyName = username, CurrentMap = 0, MyIndex = i, MySprite = sprite};
            if (isLocal)
            {
                Globals.Entities[i].IsLocal = true;
            }
        }

        public static void AddEvent(int index, string username, string sprite, bool isLocal)
        {
            var i = index;
            while (Globals.Events.Count <= index)
            {
                Globals.Events.Add(null);
            }
            if (Globals.Events[i] != null) { RemoveEntity(i, 1); }
            Globals.Events[i] = new Player {MyName = username, CurrentMap = 0, MySprite = sprite};
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
