using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;

using Intersect.Utilities;

namespace Intersect.Graphics;

/// <summary>
/// Describes a 32-bit ABGR packed color.
/// </summary>
[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public partial struct Color : IEquatable<Color>
{
    private const uint MaskA = 0xff000000;
    private const uint MaskB = 0x00ff0000;
    private const uint MaskG = 0x0000ff00;
    private const uint MaskR = 0x000000ff;
    private const uint MaskBGR = MaskB | MaskG | MaskR;

    /// <summary>
    /// Gets or sets packed value of this <see cref="Color"/>.
    /// </summary>
    public uint PackedValue;

    /// <summary>
    /// Constructs an RGBA color from a packed value.
    /// The value is a 32-bit unsigned integer, with R in the least significant octet.
    /// </summary>
    /// <param name="packedValue">The packed value.</param>
    public Color(uint packedValue)
    {
        PackedValue = packedValue;
    }

    /// <summary>
    /// Constructs an RGBA color from the XYZW unit length components of a vector.
    /// </summary>
    /// <param name="color">A <see cref="Vector4"/> representing color.</param>
    public Color(Vector4 color)
        : this((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255), (int)(color.W * 255))
    {
    }

    /// <summary>
    /// Constructs an RGBA color from the XYZ unit length components of a vector. Alpha value will be opaque.
    /// </summary>
    /// <param name="color">A <see cref="Vector3"/> representing color.</param>
    public Color(Vector3 color)
        : this((int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255))
    {
    }

    /// <summary>
    /// Constructs an RGBA color from a <see cref="Color"/> and an alpha value.
    /// </summary>
    /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
    /// <param name="alpha">The alpha component value from 0 to 255.</param>
    public Color(Color color, int alpha)
    {
        if ((alpha & 0xFFFFFF00) != 0)
        {
            var clampedA = (uint)MathHelper.Clamp(alpha, byte.MinValue, byte.MaxValue);

            PackedValue = (color.PackedValue & MaskBGR) | (clampedA << 24);
        }
        else
        {
            PackedValue = (color.PackedValue & MaskBGR) | ((uint)alpha << 24);
        }
    }

    /// <summary>
    /// Constructs an RGBA color from color and alpha value.
    /// </summary>
    /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
    /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
    public Color(Color color, float alpha) :
        this(color, (int)(alpha * 255))
    {
    }

    /// <summary>
    /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
    /// </summary>
    /// <param name="r">Red component value from 0.0f to 1.0f.</param>
    /// <param name="g">Green component value from 0.0f to 1.0f.</param>
    /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
    public Color(float r, float g, float b)
        : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
    {
    }

    /// <summary>
    /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
    /// </summary>
    /// <param name="r">Red component value from 0.0f to 1.0f.</param>
    /// <param name="g">Green component value from 0.0f to 1.0f.</param>
    /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
    /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
    public Color(float r, float g, float b, float alpha)
        : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
    {
    }

    /// <summary>
    /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
    /// </summary>
    /// <param name="r">Red component value from 0 to 255.</param>
    /// <param name="g">Green component value from 0 to 255.</param>
    /// <param name="b">Blue component value from 0 to 255.</param>
    public Color(int r, int g, int b)
    {
        PackedValue = 0xFF000000; // A = 255

        if (((r | g | b) & 0xFFFFFF00) != 0)
        {
            var clampedR = (uint)MathHelper.Clamp(r, byte.MinValue, byte.MaxValue);
            var clampedG = (uint)MathHelper.Clamp(g, byte.MinValue, byte.MaxValue);
            var clampedB = (uint)MathHelper.Clamp(b, byte.MinValue, byte.MaxValue);

            PackedValue |= (clampedB << 16) | (clampedG << 8) | (clampedR);
        }
        else
        {
            PackedValue |= ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
        }
    }

    /// <summary>
    /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
    /// </summary>
    /// <param name="r">Red component value from 0 to 255.</param>
    /// <param name="g">Green component value from 0 to 255.</param>
    /// <param name="b">Blue component value from 0 to 255.</param>
    /// <param name="alpha">Alpha component value from 0 to 255.</param>
    public Color(int r, int g, int b, int alpha)
    {
        if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
        {
            var clampedR = (uint)MathHelper.Clamp(r, byte.MinValue, byte.MaxValue);
            var clampedG = (uint)MathHelper.Clamp(g, byte.MinValue, byte.MaxValue);
            var clampedB = (uint)MathHelper.Clamp(b, byte.MinValue, byte.MaxValue);
            var clampedA = (uint)MathHelper.Clamp(alpha, byte.MinValue, byte.MaxValue);

            PackedValue = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | (clampedR);
        }
        else
        {
            PackedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
        }
    }

    /// <summary>
    /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
    /// </summary>
    /// <remarks>
    /// This overload sets the values directly without clamping, and may therefore be faster than the other overloads.
    /// </remarks>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="alpha"></param>
    public Color(byte r, byte g, byte b, byte alpha)
    {
        PackedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (r);
    }

    /// <summary>
    /// Gets or sets the alpha component.
    /// </summary>
    [DataMember]
    public byte A
    {
        get => unchecked((byte)(PackedValue >> 24));
        set => PackedValue = (PackedValue & ~MaskA) | ((uint)value << 24);
    }

    /// <summary>
    /// Gets or sets the blue component.
    /// </summary>
    [DataMember]
    public byte B
    {
        get => unchecked((byte)(PackedValue >> 16));
        set => PackedValue = (PackedValue & ~MaskB) | ((uint)value << 16);
    }

    /// <summary>
    /// Gets or sets the green component.
    /// </summary>
    [DataMember]
    public byte G
    {
        get => unchecked((byte)(PackedValue >> 8));
        set => PackedValue = (PackedValue & ~MaskG) | ((uint)value << 8);
    }

    /// <summary>
    /// Gets or sets the red component.
    /// </summary>
    [DataMember]
    public byte R
    {
        get => unchecked((byte)PackedValue);
        set => PackedValue = (PackedValue & ~MaskR) | value;
    }

    /// <summary>
    /// Gets the hash code of this <see cref="Color"/>.
    /// </summary>
    /// <returns>Hash code of this <see cref="Color"/>.</returns>
    public override int GetHashCode() => PackedValue.GetHashCode();

    /// <summary>
    /// Compares whether current instance is equal to specified <see cref="Color"/>.
    /// </summary>
    /// <param name="other">The <see cref="Color"/> to compare.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public bool Equals(Color other) => PackedValue == other.PackedValue;

    /// <summary>
    /// Compares whether current instance is equal to specified object.
    /// </summary>
    /// <param name="obj">The <see cref="Color"/> to compare.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public override bool Equals(object obj) => obj is Color color && Equals(color);

    /// <summary>
    /// Performs linear interpolation of <see cref="Color"/>.
    /// </summary>
    /// <param name="value1">Source <see cref="Color"/>.</param>
    /// <param name="value2">Destination <see cref="Color"/>.</param>
    /// <param name="amount">Interpolation factor.</param>
    /// <returns>Interpolated <see cref="Color"/>.</returns>
    public static Color Lerp(Color value1, Color value2, float amount)
    {
        amount = MathHelper.Clamp(amount, 0, 1);
        return new Color(
            (int)MathHelper.Lerp(value1.R, value2.R, amount),
            (int)MathHelper.Lerp(value1.G, value2.G, amount),
            (int)MathHelper.Lerp(value1.B, value2.B, amount),
            (int)MathHelper.Lerp(value1.A, value2.A, amount));
    }

    /// <summary>
    /// <see cref="Color.Lerp"/> should be used instead of this function.
    /// </summary>
    /// <returns>Interpolated <see cref="Color"/>.</returns>
    [Obsolete("Color.Lerp should be used instead of this function.")]
    public static Color LerpPrecise(Color value1, Color value2, float amount)
    {
        amount = MathHelper.Clamp(amount, 0, 1);
        return new Color(
            (int)MathHelper.LerpPrecise(value1.R, value2.R, amount),
            (int)MathHelper.LerpPrecise(value1.G, value2.G, amount),
            (int)MathHelper.LerpPrecise(value1.B, value2.B, amount),
            (int)MathHelper.LerpPrecise(value1.A, value2.A, amount));
    }

    /// <summary>
    /// Multiply <see cref="Color"/> by value.
    /// </summary>
    /// <param name="value">Source <see cref="Color"/>.</param>
    /// <param name="scale">Multiplicator.</param>
    /// <returns>Multiplication result.</returns>
    public static Color Multiply(Color value, float scale) => value * scale;

    /// <summary>
    /// Compares whether two <see cref="Color"/> instances are equal.
    /// </summary>
    /// <param name="a"><see cref="Color"/> instance on the left of the equal sign.</param>
    /// <param name="b"><see cref="Color"/> instance on the right of the equal sign.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public static bool operator ==(Color a, Color b) => a.Equals(b);

    /// <summary>
    /// Compares whether two <see cref="Color"/> instances are not equal.
    /// </summary>
    /// <param name="a"><see cref="Color"/> instance on the left of the not equal sign.</param>
    /// <param name="b"><see cref="Color"/> instance on the right of the not equal sign.</param>
    /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
    public static bool operator !=(Color a, Color b) => !a.Equals(b);

    /// <summary>
    /// Multiply <see cref="Color"/> by value.
    /// </summary>
    /// <param name="value">Source <see cref="Color"/>.</param>
    /// <param name="scale">Multiplicator.</param>
    /// <returns>Multiplication result.</returns>
    public static Color operator *(Color value, float scale)
    {
        return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
    }

    public static Color operator *(float scale, Color value)
    {
        return new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
    }

    /// <summary>
    /// Gets a <see cref="Vector3"/> representation for this object.
    /// </summary>
    /// <returns>A <see cref="Vector3"/> representation for this object.</returns>
    public Vector3 ToVector3() => new Vector3(R, G, B) / 255.0f;

    /// <summary>
    /// Gets a <see cref="Vector4"/> representation for this object.
    /// </summary>
    /// <returns>A <see cref="Vector4"/> representation for this object.</returns>
    public Vector4 ToVector4() => new Vector4(R, G, B, A) / 255.0f;

    /// <summary>
    /// Converts the internal packed ABGR value to packed ARGB.
    /// </summary>
    /// <returns>packed ARGB value</returns>
    public uint ToPackedARGB() =>
        (MaskA & PackedValue) |
        ((MaskB & PackedValue) >> 16) |
        (MaskG & PackedValue) |
        ((MaskR & PackedValue) << 16);

    /// <summary>
    /// Converts the internal packed RGBA value to packed ARGB.
    /// </summary>
    /// <returns>packed RGBA value</returns>
    public uint ToPackedRGBA() =>
        ((MaskA & PackedValue) >> 24) |
        ((MaskB & PackedValue) >> 8) |
        ((MaskG & PackedValue) << 8) |
        ((MaskR & PackedValue) << 24);

    internal string DebugDisplayString => $"rgba({R}, {G}, {B}, {A})";

    /// <summary>
    /// Returns a <see cref="string"/> representation of this <see cref="Color"/> in the format:
    /// {R:[red] G:[green] B:[blue] A:[alpha]}
    /// </summary>
    /// <returns><see cref="string"/> representation of this <see cref="Color"/>.</returns>
    public override string ToString() => $"rgba({R}, {G}, {B}, {A})";

    /// <summary>
    /// Translate a non-premultipled alpha <see cref="Color"/> to a <see cref="Color"/> that contains premultiplied alpha.
    /// </summary>
    /// <param name="vector">A <see cref="Vector4"/> representing color.</param>
    /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
    public static Color FromNonPremultiplied(Vector4 vector)
    {
        return new Color(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);
    }

    /// <summary>
    /// Translate a non-premultipled alpha <see cref="Color"/> to a <see cref="Color"/> that contains premultiplied alpha.
    /// </summary>
    /// <param name="r">Red component value.</param>
    /// <param name="g">Green component value.</param>
    /// <param name="b">Blue component value.</param>
    /// <param name="a">Alpha component value.</param>
    /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
    public static Color FromNonPremultiplied(int r, int g, int b, int a)
    {
        return new Color(r * a / 255, g * a / 255, b * a / 255, a);
    }

    /// <summary>
    /// Deconstruction method for <see cref="Color"/>.
    /// </summary>
    /// <param name="r">Red component value from 0 to 255.</param>
    /// <param name="g">Green component value from 0 to 255.</param>
    /// <param name="b">Blue component value from 0 to 255.</param>
    public void Deconstruct(out byte r, out byte g, out byte b)
    {
        r = R;
        g = G;
        b = B;
    }

    /// <summary>
    /// Deconstruction method for <see cref="Color"/>.
    /// </summary>
    /// <param name="r">Red component value from 0.0f to 1.0f.</param>
    /// <param name="g">Green component value from 0.0f to 1.0f.</param>
    /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
    public void Deconstruct(out float r, out float g, out float b)
    {
        r = R / 255f;
        g = G / 255f;
        b = B / 255f;
    }

    /// <summary>
    /// Deconstruction method for <see cref="Color"/> with Alpha.
    /// </summary>
    /// <param name="r">Red component value from 0 to 255.</param>
    /// <param name="g">Green component value from 0 to 255.</param>
    /// <param name="b">Blue component value from 0 to 255.</param>
    /// <param name="a">Alpha component value from 0 to 255.</param>
    public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
    {
        r = R;
        g = G;
        b = B;
        a = A;
    }

    /// <summary>
    /// Deconstruction method for <see cref="Color"/> with Alpha.
    /// </summary>
    /// <param name="r">Red component value from 0.0f to 1.0f.</param>
    /// <param name="g">Green component value from 0.0f to 1.0f.</param>
    /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
    /// <param name="a">Alpha component value from 0.0f to 1.0f.</param>
    public void Deconstruct(out float r, out float g, out float b, out float a)
    {
        r = R / 255f;
        g = G / 255f;
        b = B / 255f;
        a = A / 255f;
    }
}
