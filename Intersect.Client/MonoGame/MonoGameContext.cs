using Intersect.Client.Framework;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Storage;
using Intersect.Client.MonoGame.Content;
using Intersect.Client.MonoGame.Graphics;
using Intersect.Client.MonoGame.Storage;
using Intersect.Logging;

namespace Intersect.Client.MonoGame
{
    internal class MonoGameContext : IGameContext
    {
        public IntersectGame Game { get; }

        /// <inheritdoc />
        public Logger Logger => Game.Context.Logger;

        /// <inheritdoc />
        public IContentManager ContentManager { get; }

        /// <inheritdoc />
        public IRenderer Renderer { get; }

        /// <inheritdoc />
        public IStorage Storage { get; }

        public MonoGameContext(IntersectGame game)
        {
            Game = game;

            ContentManager = new MonoContentManager(this);
            Renderer = new MonoGameRenderer(this);
            Storage = new MonoStorage(this);
        }
    }
}
