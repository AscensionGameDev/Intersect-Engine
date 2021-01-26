using System.Collections.Generic;

using Intersect.Localization;
using MessagePack;

namespace Intersect
{
    [MessagePackObject]
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

        private const float RANGE__2_X = 2 * 255;

        public Color() : this(0)
        {
        }

        public Color(int argb) : this((argb >> 24) & 0xFF, (argb >> 16) & 0xFF, (argb >> 8) & 0xFF, (argb >> 0) & 0xFF)
        {
        }

        public Color(int r, int g, int b) : this(255, r, g, b)
        {
        }

        public Color(int a, int r, int g, int b)
        {
            A = (byte) a;
            R = (byte) r;
            G = (byte) g;
            B = (byte) b;
        }

        [Key(0)]
        public byte A { get; set; }

        [Key(1)]
        public byte R { get; set; }

        [Key(2)]
        public byte G { get; set; }

        [Key(3)]
        public byte B { get; set; }

        //public float Hue
        //{
        //    get
        //    {
        //        var max = Math.Max(Math.Max(R, G), B);
        //        var min = Math.Min(Math.Min(R, G), B);
        //        if (max == min) return 0;

        //        float hue, delta = max - min;
        //        if (max == R) hue = (G - B) / delta + (G < B ? 6 : 0);
        //        else if (max == G) hue = (B - R) / delta + 2;
        //        else hue = (R - G) / delta + 4;
        //        return hue / 6f;
        //    }
        //}

        //public float Saturation
        //{
        //    get
        //    {
        //        var max = Math.Max(Math.Max(R, G), B);
        //        var min = Math.Min(Math.Min(R, G), B);
        //        if (max == min) return 0;

        //        float delta = max - min, lightness = (max + min) / RANGE__2_X;

        //        return lightness > 0.5 ? delta / (RANGE__2_X - max - min) : delta / (max + min);
        //    }
        //}

        //public float Lightness
        //{
        //    get
        //    {
        //        var max = Math.Max(Math.Max(R, G), B);
        //        var min = Math.Min(Math.Min(R, G), B);
        //        return (max + min) / RANGE__2_X;
        //    }
        //}

        public static Color Transparent => new Color(0, 0, 0, 0);

        public static Color White => new Color(255, 255, 255);

        public static Color Black => new Color(0, 0, 0);

        public static Color Red => new Color(255, 0, 0);

        public static Color Green => new Color(0, 255, 0);

        public static Color Blue => new Color(0, 0, 255);

        public static Color Yellow => new Color(255, 255, 0);

        public static Color LightCoral => new Color(240, 128, 128);

        public static Color ForestGreen => new Color(34, 139, 34);

        public static Color Magenta => new Color(255, 0, 255);

        public static Color OrangeRed => new Color(255, 69, 0);

        public static Color Orange => new Color(255, 165, 0);

        public static Color Gray => new Color(128, 128, 128);

        public static Color Cyan => new Color(0, 255, 255);

        public static Color Pink => new Color(255, 192, 203);

        public static Color FromName(string name, IDictionary<int, LocalizedString> colors)
        {
            if (name == "Black" || name == colors[0])
            {
                return Black;
            }
            else if (name == "White" || name == colors[1])
            {
                return White;
            }
            else if (name == "Pink" || name == colors[2])
            {
                return Pink;
            }
            else if (name == "Blue" || name == colors[3])
            {
                return Blue;
            }
            else if (name == "Red" || name == colors[4])
            {
                return Red;
            }
            else if (name == "Green" || name == colors[5])
            {
                return Green;
            }
            else if (name == "Yellow" || name == colors[6])
            {
                return Yellow;
            }
            else if (name == "Orange" || name == colors[7])
            {
                return Orange;
            }
            else if (name == "Purple" || name == colors[8])
            {
                return Magenta;
            }
            else if (name == "Gray" || name == colors[9])
            {
                return Gray;
            }
            else if (name == "Cyan" || name == colors[10])
            {
                return Cyan;
            }

            return White;
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(a, r, g, b);
        }

        public static Color FromArgb(int r, int g, int b)
        {
            return new Color(255, r, g, b);
        }

        public int ToArgb()
        {
            return (int) ((uint) A << 24) + (R << 16) + (G << 8) + B;
        }

        public int ToRgba()
        {
            return (int) ((uint) R << 24) + (G << 16) + (B << 8) + A;
        }

        public static Color FromArgb(int argb)
        {
            return FromArgb((argb >> 24) & 0x0FF, (argb >> 16) & 0x0FF, (argb >> 8) & 0x0FF, argb & 0x0FF);
        }

        public static Color FromRgba(int rgba)
        {
            return FromArgb((rgba >> 0) & 0x0FF, (rgba >> 24) & 0x0FF, (rgba >> 16) & 0x0FF, (rgba >> 8) & 0x0FF);
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
            if (string.IsNullOrEmpty(val))
            {
                return defaultColor;
            }

            var strs = val.Split(",".ToCharArray());
            var parts = new int[strs.Length];
            for (var i = 0; i < strs.Length; i++)
            {
                parts[i] = int.Parse(strs[i]);
            }

            return new Color(parts[0], parts[1], parts[2], parts[3]);
        }

        public static implicit operator Color(string colorString)
        {
            return FromString(colorString);
        }

    }

}
