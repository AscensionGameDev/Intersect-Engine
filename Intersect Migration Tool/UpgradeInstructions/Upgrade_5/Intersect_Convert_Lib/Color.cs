namespace Intersect.Migration.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib
{
    public class Color
    {
        public enum ChatColor
        {
            Black = 0,
            White,
            Blue,
            Red,
            Green,
            Yellow,
            Orange,
            Purple,
            Gray,
            Cyan,
            Pink,
        }

        public Color()
        {
        }

        public Color(int a, int r, int g, int b)
        {
            A = (byte) a;
            R = (byte) r;
            G = (byte) g;
            B = (byte) b;
        }

        public Color(int r, int g, int b)
        {
            A = 255;
            R = (byte) r;
            G = (byte) g;
            B = (byte) b;
        }

        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static Color White => new Color(255, 255, 255, 255);

        public static Color Black => new Color(255, 0, 0, 0);

        public static Color Transparent => new Color(0, 0, 0, 0);

        public static Color Red => new Color(255, 255, 0, 0);

        public static Color Green => new Color(255, 0, 255, 0);

        public static Color Blue => new Color(255, 0, 0, 255);

        public static Color Yellow => new Color(255, 255, 255, 0);

        public static Color LightCoral => new Color(255, 240, 128, 128);

        public static Color ForestGreen => new Color(255, 34, 139, 34);
        public static Color Magenta => new Color(255, 255, 0, 255);
        public static Color OrangeRed => new Color(255, 255, 69, 0);
        public static Color Orange => new Color(255, 255, 165, 0);
        public static Color Gray => new Color(255, 128, 128, 128);
        public static Color Cyan => new Color(255, 0, 255, 255);
        public static Color Pink => new Color(255, 255, 192, 203);

        public byte GetHue()
        {
            return 0;
        }

        public static Color FromName(string name)
        {
            switch (name)
            {
                case "Black":
                    return Black;
                case "White":
                    return White;
                case "Blue":
                    return Blue;
                case "Red":
                    return Red;
                case "Green":
                    return Green;
                case "Yellow":
                    return Yellow;
                case "Orange":
                    return Orange;
                case "Purple":
                    return Magenta;
                case "Gray":
                    return Gray;
                case "Cyan":
                    return Cyan;
                case "Pink":
                    return Pink;
                default:
                    return White;
            }
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(a, r, g, b);
        }

        public static Color FromArgb(int r, int g, int b)
        {
            return new Color(255, r, g, b);
        }
    }
}