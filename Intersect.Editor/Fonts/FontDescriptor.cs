namespace Intersect.Editor.Fonts;

public class FontDescriptor
{
    public string Name { get; set; }

    public List<GlyphRange> GlyphRanges { get; set; }

    public List<ushort> LooseGlyphs { get; set; }

    public FontDescriptor Clean()
    {
        var sortedGlyphRanges = GlyphRanges.OrderBy(range => range.Start).ToList();
        var sortedLooseGlyphs = LooseGlyphs
            .Where(glyph => !sortedGlyphRanges.Any(range => range.Contains(glyph)))
            .OrderBy(glyph => glyph)
            .ToList();

        var fontDescriptor = new FontDescriptor
        {
            Name = Name,
            GlyphRanges = sortedGlyphRanges,
            LooseGlyphs = sortedLooseGlyphs,
        };

        return fontDescriptor;
    }
}
