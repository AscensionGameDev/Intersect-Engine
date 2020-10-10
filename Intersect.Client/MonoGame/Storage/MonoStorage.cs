using Intersect.Client.Core;
using Intersect.Client.Framework;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Storage;
using Intersect.Client.MonoGame.Database;
using Intersect.Configuration;

using System.IO;

namespace Intersect.Client.MonoGame.Storage
{
    public class MonoStorage : GameStorage
    {
        /// <inheritdoc />
        public MonoStorage(IGameContext gameContext) : base(gameContext)
        {
            Configuration = new ClientConfiguration();
            Preferences = new MonoPreferences(gameContext);
        }

        /// <inheritdoc />
        public override Stream OpenContentRead(ContentType contentType, string contentName) =>
            throw new System.NotImplementedException();

        /// <inheritdoc />
        public override Stream OpenEmbeddedResourceRead(string embeddedResourceName) =>
            typeof(ClientContext).Assembly.GetManifestResourceStream(embeddedResourceName);

        /// <inheritdoc />
        public override Stream OpenFileRead(string filePath) => File.OpenRead(filePath);

        /// <inheritdoc />
        public override Stream OpenFileWrite(string filePath, bool createIfMissing = true)
        {
            if (!createIfMissing)
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(
                        $"File does not exist and {nameof(OpenFileWrite)} was called with {nameof(createIfMissing)} set to false.",
                        filePath
                    );
                }
            }

            return File.OpenWrite(filePath);
        }
    }
}
