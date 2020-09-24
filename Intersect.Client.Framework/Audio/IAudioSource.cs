using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Audio
{
    public interface IAudioSource : IAsset
    {
        AudioType AudioType { get; }

        IAudioInstance CreateInstance();
    }
}
