using System.Collections.Immutable;

namespace Intersect.Client.Framework.Graphics;

public abstract partial class Font<TPlatformObject>(string fontName, ICollection<int> supportedSizes) : IFont
{
    private readonly Dictionary<int, FontSizeRenderer<TPlatformObject>> _renderers = [];
    private readonly ImmutableSortedSet<int> _supportedSizes = supportedSizes.ToImmutableSortedSet();

    public string Name { get; } = fontName;

    public ICollection<int> SupportedSizes => _supportedSizes;

    public int PickBestMatchFor(int size)
    {
        if (_supportedSizes.Contains(size))
        {
            return size;
        }

        int bestMatch = 0, lastDelta = int.MaxValue;
        foreach (var supportedSize in supportedSizes)
        {
            var delta = Math.Abs(supportedSize - size);
            if (delta > lastDelta)
            {
                continue;
            }

            if (delta == lastDelta && supportedSize < bestMatch)
            {
                continue;
            }

            bestMatch = supportedSize;
            lastDelta = delta;
        }

        if (bestMatch < 1)
        {
            throw new InvalidOperationException($"No suitable font size found for {Name}");
        }

        return bestMatch;
    }

    public int GetNextFontSize(int startSize, int direction, int limit = 0)
    {
        int fontSize = startSize, lastDelta = 0;
        limit = Math.Abs(limit);
        foreach (var supportedSize in SupportedSizes)
        {
            var delta = supportedSize - startSize;
            if (delta != direction)
            {
                continue;
            }

            var absDelta = Math.Abs(delta);
            if (absDelta > limit)
            {
                continue;
            }

            if (absDelta < lastDelta)
            {
                continue;
            }

            fontSize = supportedSize;
            lastDelta = absDelta;
        }

        return fontSize;
    }

    public FontSizeRenderer<TPlatformObject> GetRendererFor(int size)
    {
        if (_renderers.TryGetValue(size, out var renderer))
        {
            return renderer;
        }

        var actualSize = PickBestMatchFor(size);
        if (_renderers.TryGetValue(actualSize, out renderer))
        {
            _renderers[size] = renderer;
            return renderer;
        }

        renderer = CreateRendererFor(actualSize);
        _renderers[size] = renderer;
        _renderers[actualSize] = renderer;
        return renderer;
    }

    protected abstract FontSizeRenderer<TPlatformObject> CreateRendererFor(int size);

    public override string ToString() => Name;
}