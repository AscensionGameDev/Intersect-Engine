using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Storage;
using Intersect.Logging;

namespace Intersect.Client.Framework
{
    public interface IGameContext
    {
        /// <summary>
        /// The logger instance for the client framework to interface with.
        /// </summary>
        Logger Logger { get; }

        IContentManager ContentManager { get; }

        IRenderer Renderer { get; }

        IStorage Storage { get; }
    }
}
