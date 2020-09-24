
using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework
{
    public interface IGameContext
    {
        IContentManager ContentManager { get; }
    }
}
