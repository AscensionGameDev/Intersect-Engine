using System.Diagnostics.CodeAnalysis;

namespace Intersect.Editor.Fonts;

public struct GlyphRange : IEquatable<GlyphRange>
{
    public ushort Start;

    public ushort End;

    public GlyphRange(ushort glyph)
        : this(glyph, glyph)
    {
    }

    public GlyphRange(ushort start, ushort end)
    {
        Start = Math.Min(start, end);
        End = Math.Max(start, end);
    }

    public GlyphRange(ushort start, ushort end, ushort additionalGlyph)
    {
        Start = Math.Min(Math.Min(start, end), additionalGlyph);
        End = Math.Max(Math.Max(start, end), additionalGlyph);
    }

    public GlyphRange(GlyphRange glyphRange, ushort additionalGlyph)
        : this(glyphRange.Start, glyphRange.End, additionalGlyph)
    {
    }

    public bool Contains(ushort glyph) => Start <= glyph && glyph <= End;

    public bool Intersects(GlyphRange glyphRange) => Contains(glyphRange.Start) || Contains(glyphRange.End);

    public bool IsAdjacentTo(GlyphRange glyphRange) => Start == glyphRange.End + 1 || End == glyphRange.Start - 1;

    public bool TryCombine(GlyphRange otherRange, out GlyphRange combinedRange)
    {
        if (!Intersects(otherRange) && !IsAdjacentTo(otherRange))
        {
            combinedRange = default;
            return false;
        }

        combinedRange = new GlyphRange(
            Math.Min(Start, otherRange.Start),
            Math.Max(End, otherRange.End)
        );

        return true;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj != null && obj is GlyphRange glyphRange && Equals(glyphRange);

    public bool Equals(GlyphRange other) => Start == other.Start && End == other.End;

    public override int GetHashCode() => HashCode.Combine(Start, End);

    public override string ToString() => $"{nameof(GlyphRange)}[{Start:X}-{End:X}";

    public static bool operator ==(GlyphRange left, GlyphRange right) => left.Equals(right);

    public static bool operator !=(GlyphRange left, GlyphRange right) => !left.Equals(right);

    public static List<GlyphRange> Condense(params GlyphRange[] glyphRanges) => Condense(glyphRanges as IEnumerable<GlyphRange>);

    public static List<GlyphRange> Condense(IEnumerable<GlyphRange> glyphRanges)
    {
        var ranges = glyphRanges.OrderBy(range => range.Start).ToList();

        for (var rangeIndex = ranges.Count - 1; rangeIndex > 0; rangeIndex--)
        {
            var currentRange = ranges[rangeIndex];
            var nextRange = ranges[rangeIndex - 1];
            if (currentRange.TryCombine(nextRange, out var combinedRange))
            {
                ranges[rangeIndex - 1] = combinedRange;
                ranges.RemoveAt(rangeIndex);
            }
        }

        return ranges;
    }

    public static List<GlyphRange> ComputeRanges(string text, params GlyphRange[] glyphRanges) => ComputeRanges(text, glyphRanges as IEnumerable<GlyphRange>);

    public static List<GlyphRange> ComputeRanges(string text, IEnumerable<GlyphRange> glyphRanges)
    {
        var ranges = glyphRanges.ToList();
        foreach (var glyph in text)
        {
            if (ranges.Any(range => range.Contains(glyph)))
            {
                continue;
            }

            var rangeIndex = ranges.FindIndex(range => range.Start - 1 == glyph || range.End + 1 == glyph);
            if (rangeIndex >= 0)
            {
                ranges[rangeIndex] = new GlyphRange(ranges[rangeIndex], glyph);
                continue;
            }

            ranges.Add(new GlyphRange(glyph, glyph));
        }

        return Condense(ranges);
    }
}

public static class GlyphRanges
{
    public static GlyphRange[] Cyrillic { get; } = new[] { new GlyphRange(0x0400, 0x04ff) };

    public static GlyphRange[] Greek { get; } = new[] { new GlyphRange(0x0370, 0x03ff) };

    public static GlyphRange[] Latin { get; } = new[] { new GlyphRange(0x0000, 0x024f) };

}
