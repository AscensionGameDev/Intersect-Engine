using System;

namespace Intersect.Client.Framework
{
    public abstract class HasGameContext : IHasGameContext
    {
        /// <inheritdoc />
        public IGameContext GameContext { get; }

        protected HasGameContext(IGameContext gameContext)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }
    }
}
