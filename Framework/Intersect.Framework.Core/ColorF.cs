using System.Runtime.InteropServices;

namespace Intersect.Framework.Core;

[StructLayout(layoutKind: LayoutKind.Sequential)]
#pragma warning disable CS0660, CS0661
public struct ColorF : IComparable<ColorF>, IEquatable<ColorF>
#pragma warning restore CS0660, CS0661
{
    public float A;
    public float R;
    public float G;
    public float B;

    public ColorF()
    {
    }

    public ColorF(float r, float g, float b, float a = 255f)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    public static ColorF FromArgb(float a, float r, float g, float b)
    {
        return new ColorF(r: r, g: g, b: b, a: a);
    }

    public static ColorF FromArgb(int argb)
    {
        return new ColorF(
            r: ((uint)argb >> 16) & 0xff,
            g: ((uint)argb >> 8) & 0xff,
            b: ((uint)argb >> 0) & 0xff,
            a: ((uint)argb >> 24) & 0xff
        );
    }

    #region Known Colors

    public static ColorF Black => new(r: 0, g: 0, b: 0, a: 255);
    public static ColorF Blue => new(r: 0, g: 0, b: 255, a: 255);
    public static ColorF ForestGreen => new(r: 34, g: 139, b: 34, a: 255);
    public static ColorF Green => new(r: 0, g: 255, b: 0, a: 255);
    public static ColorF LightCoral => new(r: 240, g: 128, b: 128, a: 255);
    public static ColorF Magenta => new(r: 255, g: 0, b: 255, a: 255);
    public static ColorF Red => new(r: 255, g: 0, b: 0, a: 255);
    public static ColorF Transparent => new(r: 0, g: 0, b: 0, a: 0);
    public static ColorF White => new(r: 255, g: 255, b: 255, a: 255);
    public static ColorF Yellow => new(r: 255, g: 255, b: 0, a: 255);

    #endregion Known Colors

    public int CompareTo(ColorF other)
    {
        var comparison = R.CompareTo(other.R);
        if (comparison != 0)
        {
            return comparison;
        }
        
        comparison = G.CompareTo(other.G);
        if (comparison != 0)
        {
            return comparison;
        }
        
        comparison = B.CompareTo(other.B);
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (comparison != 0)
        {
            return comparison;
        }

        return A.CompareTo(other.A);
    }

    public bool Equals(ColorF other) =>
        R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A);

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public override bool Equals(object obj) => obj is ColorF other && Equals(other);
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

    public static bool operator ==(ColorF left, ColorF right) => left.Equals(right);

    public static bool operator !=(ColorF left, ColorF right) => !(left == right);
}