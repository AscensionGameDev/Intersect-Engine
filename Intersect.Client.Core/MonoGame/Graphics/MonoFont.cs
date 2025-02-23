using System.Diagnostics;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics;

public partial class MonoFont : Font<SpriteFont>, IFont
{
    private readonly SortedDictionary<int, FontReference> _fontsBySize = [];

    public MonoFont(string fontName, IDictionary<int, FileInfo> fontSourcesBySize, ContentManager contentManager) : base(
        fontName: fontName,
        supportedSizes: fontSourcesBySize.Keys
    )
    {
        foreach (var (size, fileInfo) in fontSourcesBySize)
        {
            _fontsBySize[size] = new FontReference(
                fontName,
                size,
                () =>
                {
                    var pathWithoutExtension = GameContentManager.RemoveExtension(fileInfo.FullName);
                    return contentManager.Load<SpriteFont>(pathWithoutExtension);
                }
            );
        }
    }

    private sealed class FontReference(string fontName, int size, Func<SpriteFont> platformObjectFactory)
    {
        private SpriteFont? _instance;

        public string FontName { get; } = fontName;
        public readonly Func<SpriteFont> PlatformObjectFactory = platformObjectFactory;
        public int Size { get; } = size;

        public SpriteFont Instance
        {
            get
            {
                try
                {
                    return _instance ??= PlatformObjectFactory();
                }
                catch (Exception exception)
                {
                    throw new ContentLoadException($"Failed to load {Size}pt source for '{FontName}'", exception);
                }
            }
        }
    }

    protected override FontSizeRenderer<SpriteFont> CreateRendererFor(int size)
    {
        if (!_fontsBySize.TryGetValue(size, out var reference))
        {
            throw new UnreachableException($"Missing reference for size {size} of '{Name}'");
        }

        var instance = reference.Instance;
        return new SpriteFontRenderer(instance);
    }
}