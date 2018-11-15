namespace Intersect.Client.Framework.GenericClasses
{
    public class Color
    {
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

        public static Color FromArgb(int argb)
        {
            return FromArgb((argb >> 24) & 0x0FF, (argb >> 16) & 0x0FF, (argb >> 8) & 0x0FF, argb & 0x0FF);
        }

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

        public byte GetHue()
        {
            return 0;
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(a, r, g, b);
        }

        public static string ToString(Color clr)
        {
            if (clr == null)
            {
                return "";
            }
            else
            {
                return clr.A + "," + clr.R + "," + clr.G + "," + clr.B;
            }
        }

        public static Color FromString(string val, Color defaultColor = null)
        {
            if (string.IsNullOrEmpty(val)) return defaultColor;
            string[] strs = val.Split(",".ToCharArray());
            int[] parts = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
                parts[i] = int.Parse(strs[i]);
            return new Color(parts[0], parts[1], parts[2], parts[3]);
        }
    }
}