//Pulled from the Open Source Mono library and modified.
//https://github.com/mono/mono

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib
{
    public class Color
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Color()
        {

        }

        public Color(int a, int r, int g, int b)
        {
            A = (byte)a;
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }

        public Color(int r, int g, int b)
        {
            A = 255;
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }

        public byte GetHue()
        {
            return 0;
        }

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

        public static Color FromName(string name)
        {
            switch (name)
            {
                case "Black":
                    return Color.Black;
                case "White":
                    return Color.White;
                case "Blue":
                    return Color.Blue;
                case "Red":
                    return Color.Red;
                case "Green":
                    return Color.Green;
                case "Yellow":
                    return Color.Yellow;
                case "Orange":
                    return Color.Orange;
                case "Purple":
                    return Color.Magenta;
                case "Gray":
                    return Color.Gray;
                case "Cyan":
                    return Color.Cyan;
                case "Pink":
                    return Color.Pink;
                default:
                    return Color.White;
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

        public static Color White
        {
            get { return new Color(255, 255, 255, 255); }
        }

        public static Color Black
        {
            get { return new Color(255, 0, 0, 0); }
        }

        public static Color Transparent
        {
            get { return new Color(0, 0, 0, 0); }
        }

        public static Color Red
        {
            get { return new Color(255, 255, 0, 0); }
        }

        public static Color Green
        {
            get { return new Color(255, 0, 255, 0); }
        }

        public static Color Blue
        {
            get { return new Color(255, 0, 0, 255); }
        }

        public static Color Yellow
        {
            get { return new Color(255, 255, 255, 0); }
        }
        public static Color LightCoral
        {
            get { return new Color(255, 240, 128, 128); }
        }
        public static Color ForestGreen
        {
            get { return new Color(255, 34, 139, 34); }
        }
        public static Color Magenta
        {
            get { return new Color(255, 255, 0, 255); }
        }
        public static Color OrangeRed
        {
            get { return new Color(255, 255, 69, 0); }
        }
        public static Color Orange
        {
            get { return new Color(255, 255,165,0); }
        }
        public static Color Gray
        {
            get { return new Color(255, 128, 128, 128); }
        }
        public static Color Cyan
        {
            get { return new Color(255, 0,255,255); }
        }
        public static Color Pink
        {
            get { return new Color(255, 255,192,203); }
        }
    }
}
