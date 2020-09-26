
using Intersect.Client.Framework;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.MonoGame.Content;
using Intersect.Client.MonoGame.Graphics;

namespace Intersect.Client.MonoGame
{
    internal class MonoGameContext : IGameContext
    {
        /// <inheritdoc />
        public IContentManager ContentManager { get; }

        /// <inheritdoc />
        public IRenderer Renderer { get; }

        public MonoGameContext(IntersectGame game)
        {
            ContentManager = new MonoContentManager(game.Content);
            Renderer = new MonoGameRenderer(game);
        }
    }
}
