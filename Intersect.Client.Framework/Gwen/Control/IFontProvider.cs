using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Gwen.Control;

public interface IFontProvider
{
    IFont? Font { get; set; }

    int FontSize { get; set; }
}