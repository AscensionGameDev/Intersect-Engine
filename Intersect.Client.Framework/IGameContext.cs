
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework
{
    public interface IGameContext
    {
        IContentManager ContentManager { get; }

        IRenderer Renderer { get; }
    }
}
