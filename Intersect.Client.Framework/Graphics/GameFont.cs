namespace Intersect.Client.Framework.Graphics
{
    public abstract class GameFont : IFont
    {
        public GameFont(string fontName, int fontSize)
        {
            FontName = fontName;
            Size = fontSize;
        }

        public string Name => $"{FontName},{Size}";

        public string FontName { get; }

        public int Size { get; }

        public abstract TFont AsPlatformFont<TFont>();

        public override string ToString() => Name;
    }
}
