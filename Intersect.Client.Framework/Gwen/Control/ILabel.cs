using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Control
{
    public interface ILabel : IColorableText
    {
        Pos Alignment { get; set; }

        bool AutoSizeToContents { get; set; }

        GameFont Font { get; set; }

        string FontName { get; set; }

        int FontSize { get; set; }

        Color TextColor { get; set; }

        Color TextColorOverride { get; set; }

        Padding TextPadding { get; set; }

        void SizeToContents();

        void UpdateColors();
    }
}
