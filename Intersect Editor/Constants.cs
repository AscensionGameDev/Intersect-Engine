using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect_Editor
{
    public static class Constants
    {
        public const int MAP_WIDTH = 30;
        public const int MAP_HEIGHT = 30;
        public const int LAYER_COUNT = 5;


        // Autotiles
        public const byte AUTO_INNER = 1;
        public const byte AUTO_OUTER = 2;
        public const byte AUTO_HORIZONTAL = 3;
        public const byte AUTO_VERTICAL = 4;
        public const byte AUTO_FILL = 5;

        // Autotile types
        public const byte AUTOTILE_NONE = 0;
        public const byte AUTOTILE_NORMAL = 1;
        public const byte AUTOTILE_FAKE = 2;
        public const byte AUTOTILE_ANIM = 3;
        public const byte AUTOTILE_CLIFF = 4;

        public const byte AUTOTILE_WATERFALL = 5;
        // Rendering
        public const byte RENDER_STATE_NONE = 0;
        public const byte RENDER_STATE_NORMAL = 1;
        public const byte RENDER_STATE_AUTOTILE = 2;

        public const int VARIABLE_COUNT = 100;
        public const int SWITCH_COUNT = 100;
    }
}
