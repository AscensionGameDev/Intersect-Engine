namespace Intersect_Editor.Classes
{
    public static class Database
    {
        public static void InitDatabase()
        {
            Globals.GameItems = new ItemStruct[Constants.MaxItems];
            Globals.GameNpcs = new NpcStruct[Constants.MaxNpcs];
            Globals.GameSpells = new SpellStruct[Constants.MaxSpells];
            Globals.GameAnimations = new AnimationStruct[Constants.MaxAnimations];
        }

    }
}
