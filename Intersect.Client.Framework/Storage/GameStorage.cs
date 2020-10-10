using Intersect.Configuration;

using System;
using System.IO;

using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Storage
{
    public abstract class GameStorage : IStorage
    {
        protected IGameContext GameContext { get; }

        protected GameStorage(IGameContext gameContext)
        {
            GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        }

        /// <inheritdoc />
        public IClientConfiguration Configuration { get; protected set; }

        /// <inheritdoc />
        public IPreferences Preferences { get; protected set; }

        /// <inheritdoc />
        public abstract Stream OpenContentRead(ContentType contentType, string contentName);

        /// <inheritdoc />
        public abstract Stream OpenEmbeddedResourceRead(string embeddedResourceName);

        /// <inheritdoc />
        public abstract Stream OpenFileRead(string filePath);

        /// <inheritdoc />
        public abstract Stream OpenFileWrite(string filePath, bool createIfMissing = true);
    }
}
