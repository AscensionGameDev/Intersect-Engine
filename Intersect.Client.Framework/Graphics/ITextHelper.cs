using System.Numerics;
using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Graphics;

public interface ITextHelper
{
    Vector2 MeasureText(string? text, IFont? font, int size, float fontScale);
}