namespace Intersect;


public partial class ColorF : IEquatable<ColorF>
{
    private const float Tolerance = 1.0f / 256.0f;

    public ColorF()
    {
    }

    public ColorF(float a, float r, float g, float b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    public ColorF(float r, float g, float b)
    {
        A = 255;
        R = r;
        G = g;
        B = b;
    }

    public float A { get; set; }

    public float R { get; set; }

    public float G { get; set; }

    public float B { get; set; }

    public static ColorF White => new(255, 255, 255, 255);

    public static ColorF Black => new(255, 0, 0, 0);

    public static ColorF Transparent => new(0, 0, 0, 0);

    public static ColorF Red => new(255, 255, 0, 0);

    public static ColorF Green => new(255, 0, 255, 0);

    public static ColorF Blue => new(255, 0, 0, 255);

    public static ColorF Yellow => new(255, 255, 255, 0);

    public static ColorF LightCoral => new(255, 240, 128, 128);

    public static ColorF ForestGreen => new(255, 34, 139, 34);

    public static ColorF Magenta => new(255, 255, 0, 255);

    public byte GetHue()
    {
        return 0;
    }

    public static ColorF FromArgb(float a, float r, float g, float b)
    {
        return new ColorF(a, r, g, b);
    }

    public override bool Equals(object other) => other is ColorF color && Equals(color);

    public bool Equals(ColorF other) => Math.Abs(R - other.R) < Tolerance && Math.Abs(G - other.G) < Tolerance &&
                                        Math.Abs(B - other.B) < Tolerance && Math.Abs(A - other.A) < Tolerance;
}
