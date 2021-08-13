using Intersect.Client.Framework.Content;

namespace Intersect.Client.Framework.Audio
{

    public abstract class GameAudioSource : IAsset
    {
        public string Name { get; set; }

        public abstract GameAudioInstance CreateInstance();

    }

}
