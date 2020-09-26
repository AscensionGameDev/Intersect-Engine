using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Graphics
{
    public interface IFont : IAsset
    {
        string FontName { get; }

        int Size { get; }

        TFont AsPlatformFont<TFont>();
    }
}
