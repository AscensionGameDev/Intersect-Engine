using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics;

public interface ITextHelper
{
    Pointf MeasureText(string? text, IFont? font, int size, float fontScale);
}