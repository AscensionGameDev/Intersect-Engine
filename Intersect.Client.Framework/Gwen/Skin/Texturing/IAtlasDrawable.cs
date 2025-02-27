using Intersect.Client.Framework.GenericClasses;

namespace Intersect.Client.Framework.Gwen.Skin.Texturing;

public interface IAtlasDrawable
{
    void Draw(Renderer.Base renderer, Rectangle targetBounds, Color color);
}