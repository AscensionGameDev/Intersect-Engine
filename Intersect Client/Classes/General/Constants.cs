namespace Intersect_Client.Classes
{
    public static class Constants
    {
        public const int MaxStatValue = 200;
        public const int MaxStats = 5;
        public const int MaxItems = 255;
        public const int MaxNpcs = 255;
        public const int MaxNpcDrops = 10;
        public const int MaxSpells = 255;
        public const int MaxAnimations = 255;

        public const int MapWidth = 30;
        public const int MapHeight = 30;
        public const int LayerCount = 5;

        //Item Rendering
        public static int ItemXPadding = 4;
        public static int ItemYPadding = 4;

        //Player Constraints
        public const int MaxInvItems = 35;
        public const int MaxPlayerSkills = 35;

        // Autotiles
        public const byte AutoInner = 1;
        public const byte AutoOuter = 2;
        public const byte AutoHorizontal = 3;
        public const byte AutoVertical = 4;
        public const byte AutoFill = 5;

        // Autotile types
        public const byte AutotileNone = 0;
        public const byte AutotileNormal = 1;
        public const byte AutotileFake = 2;
        public const byte AutotileAnim = 3;
        public const byte AutotileCliff = 4;

        public const byte AutotileWaterfall = 5;
        // Rendering
        public const byte RenderStateNone = 0;
        public const byte RenderStateNormal = 1;
        public const byte RenderStateAutotile = 2;

        public const int VariableCount = 100;
        public const int SwitchCount = 100;
    }
}
