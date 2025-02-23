using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Control;

public interface ILabel : IAutoSizeToContents, IColorableText, ITextContainer
{
    Pos TextAlign { get; set; }

    IFont? Font { get; set; }

    string? FontName { get; set; }

    int FontSize { get; set; }

    /// <summary>
    /// Resizes the component to fit the contents.
    /// </summary>
    /// <returns>If the size was changed.</returns>
    bool SizeToContents();

    void UpdateColors();
}