using Intersect.Client.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Client.MonoGame.Graphics;

public sealed class SpriteFontRenderer(SpriteFont platformObject) : FontSizeRenderer<SpriteFont>(platformObject)
{
}