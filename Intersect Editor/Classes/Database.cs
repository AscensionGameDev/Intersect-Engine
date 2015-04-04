namespace Intersect_Editor.Classes
{
    public static class Database
    {
        public static void InitDatabase()
        {
            InitItems();
        }
        private static void InitItems()
        {
            Globals.Items = new ItemStruct[Constants.MaxItems];
        }
    }
}
