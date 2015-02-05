namespace Intersect_Editor.Classes
{
    public static class Constants
    {
        public const int MaxStats = 5;
        public const int MaxItems = 255;

        public const int MapWidth = 30;
        public const int MapHeight = 30;
        public const int LayerCount = 5;


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
